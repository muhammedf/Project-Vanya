import { createSignal, For } from "solid-js";
import { Link } from '@solidjs/router'

interface Instrument {
    id: number,
    name: string
}

var instruments: Instrument[] = [{ id: 1, name: "VanyaCoin" }, { id: 2, name: "DogeCoin" }, { id: 3, name: "BitCoin" }]
// await fetch("https://localhost:7104/api/Instrument").then(response => {
// console.log(response)    
// if(response.ok)
//         return response.json() as Promise<Instrument[]>
//     throw new Error("Not Okay!")
//})

console.log(instruments)

export const InstrumentList = () => {
    return (
        <div>
            <For each={instruments}>
                {(item, index) => (
                    <Link href={"/market/" + item.id}>{item.name}</Link>
                )}
            </For>
        </div>
    )
}