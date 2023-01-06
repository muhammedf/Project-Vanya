import * as model from './model'

var baseurl = "https://localhost:7104/api";

async function fetchi<T>(req: string): Promise<T> {
    const r = await fetch(baseurl + "/" + req);
    return r.json() as T;
}

const api = {
    getBoard: async function (instid: string): Promise<model.Board> {
        var board = await fetchi<model.Board>("Trading/get_board" + "?instId=" + instid);
        console.log(board)
        return board
    }
}

export default api