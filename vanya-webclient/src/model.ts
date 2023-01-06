export enum BidAsk {
    Bid,
    Ask
}

export interface Deal
{
    price: number,
    askOrder: Order,
    bidOrder: Order
}

export class Order {
    price: number
    quantity: number
    bidAsk: BidAsk
    private dirty : boolean

    constructor(p: number, q: number, ba: BidAsk){
        this.price = p
        this.quantity = q
        this.bidAsk = ba
        this.dirty = false
    }

    isDirty() : boolean {
        let tempdirty = this.dirty
        this.dirty = false
        return tempdirty
    }

    makeDirty():void{
        this.dirty = true
    }
}

export class Board {
    bidOrders: Order[]
    askOrders: Order[]
    hash: string
    constructor(){
        this.bidOrders = []
        this.askOrders = []
        this.hash = "null"
    }
}

export type Map<Key extends string | number | symbol, Value> = {
    [key in Key]: Value | undefined
}

export enum BoardEventType
{
    AddOrder, RemoveOrder, NewDeal
}

export interface BoardEvent{
    eventType: BoardEventType,
    eventData: Order | Deal,
    oldTrackingHash: string,
    newTrackingHash: string
}