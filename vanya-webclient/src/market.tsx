import { useParams } from '@solidjs/router'
import api from './api'
import { createEffect, createSignal, onMount, onCleanup, onError, For } from 'solid-js'
import * as model from './model'
import styles from './App.module.css';
import con from './signalcon'

function increaseQuantiy(board: model.Board, bidAsk: model.BidAsk, price: number, increase: number) {
    let orders = bidAsk === model.BidAsk.Bid ? board.bidOrders : board.askOrders
    let index = orders.findIndex(x => x.price == price)
    if (index != -1)
        orders[index].quantity += increase
    else {
        let factor = bidAsk === model.BidAsk.Bid ? 1 : -1
        index = orders.findIndex(x => factor * x.price < factor * price)
        index = index == -1 ? orders.length : index
        orders.splice(index, 0, new model.Order(price, increase, bidAsk))
    }
    orders[index].makeDirty()
}

function decreaseQuantiy(board: model.Board, bidAsk: model.BidAsk, price: number, decrease: number) {
    let orders = bidAsk === model.BidAsk.Bid ? board.bidOrders : board.askOrders
    let index = orders.findIndex(x => x.price == price)
    if (index != -1)
        orders[index].quantity -= decrease
    else{
        throw new Error("inconsistent data")
    }

    if (orders[index].quantity == 0)
        orders.splice(index, 1)

    orders[index].makeDirty()
}

export const Market = () => {

    var params = useParams()
    onMount(async () => {
        let newBoard = await getBoard(params.id)
        setBoard(newBoard)
        con.send("sub", params.id)
    })
    onCleanup(() => {
        con.send("unsub", params.id)
        console.log("Im goiiiiiiinggg :)))")
    })
    onError(er => console.log("errorrrrr", er))

    const [board, setBoard] = createSignal<model.Board>(new model.Board(), { equals: false })
    const [depth, setDepth] = createSignal<Depth[]>([])
    createEffect(() => {
        let boardData = board()
        let maxi = Math.max(boardData.askOrders.length, boardData.bidOrders.length)
        let depthData: Depth[] = []
        for (let index = 0; index < maxi; index++) {
            const sell = boardData.askOrders[index]
            const buy = boardData.bidOrders[index]
            depthData.push({ buy: buy, sell: sell })
        }
        setDepth(depthData)
    })

    con.on("change", data => {
        var boardEvent: model.BoardEvent = data
        switch (boardEvent.eventType) {
            case model.BoardEventType.AddOrder:
                setBoard(board => {

                    if(board.hash !== boardEvent.oldTrackingHash){
                        throw new Error("Upsie dupsie")
                    }

                    board.hash = boardEvent.newTrackingHash

                    let eventData: model.Order = boardEvent.eventData as model.Order
                    increaseQuantiy(board, eventData.bidAsk, eventData.price, eventData.quantity)
                    return board
                })
                break;
            case model.BoardEventType.NewDeal:
                setBoard(board => {

                    if(board.hash !== boardEvent.oldTrackingHash){
                        throw new Error("Upsie dupsie")
                    }

                    board.hash = boardEvent.newTrackingHash

                    let eventData: model.Deal = boardEvent.eventData as model.Deal
                    decreaseQuantiy(board, model.BidAsk.Ask, eventData.askOrder.price, eventData.askOrder.quantity)
                    decreaseQuantiy(board, model.BidAsk.Bid, eventData.bidOrder.price, eventData.bidOrder.quantity)
                    return board
                })
            default:
                break;
        }
    })


    return (
        <div class={styles.market}>
            <For each={depth()}>
                {(item, index) => (
                    <div class={styles.depth}>
                        <div class={styles.sell} classList={{ [styles.anime]: item.sell?.isDirty() }}>
                            <div class={styles.inside}>
                                <div class={styles.quantity}>{item.sell?.quantity}</div>
                                <div class={styles.price}>{item.sell?.price}</div>
                            </div>
                        </div>
                        <div class={styles.buy} classList={{ [styles.anime]: item.buy?.isDirty() }}>
                            <div class={styles.inside}>
                                <div class={styles.price}>{item.buy?.price}</div>
                                <div class={styles.quantity}>{item.buy?.quantity}</div>
                            </div>
                        </div>
                    </div>
                )}
            </For>
        </div>
    )
}

interface Depth {
    sell: model.Order | undefined,
    buy: model.Order | undefined
}

type OMap = model.Map<number, model.Order>

async function getBoard(id: string): Promise<model.Board> {
    var board = await api.getBoard(id)
    var askmap: OMap = {}
    var bidmap: OMap = {}

    board.askOrders.forEach(element => {
        var price = element.price
        var order = askmap[price]
        if (order != undefined) {
            order.quantity += element.quantity
        }
        else
            askmap[price] = new model.Order(price, element.quantity, model.BidAsk.Ask)
    });

    board.bidOrders.forEach(element => {
        var price = element.price
        var order = bidmap[price]
        if (order != undefined) {
            order.quantity += element.quantity
        }
        else
            bidmap[price] = new model.Order(price, element.quantity, model.BidAsk.Bid)
    });

    var zippedBoard: model.Board = new model.Board()
    zippedBoard.hash = board.hash

    for (const key in askmap) {
        if (Object.prototype.hasOwnProperty.call(askmap, key)) {
            const element = askmap[key];
            if (element != undefined)
                zippedBoard.askOrders.push(element)
        }
    }

    for (const key in bidmap) {
        if (Object.prototype.hasOwnProperty.call(bidmap, key)) {
            const element = bidmap[key];
            if (element != undefined)
                zippedBoard.bidOrders.push(element)
        }
    }

    zippedBoard.askOrders.sort((a, b) => a.price - b.price)
    zippedBoard.bidOrders.sort((a, b) => -(a.price - b.price))

    return zippedBoard
}