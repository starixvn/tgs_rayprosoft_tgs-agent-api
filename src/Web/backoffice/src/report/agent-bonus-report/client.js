import useSWR, { cache } from "swr"
import {
    urlWith,
    useGet,
    usePostData,
    usePut,
    usePutData,
    usePostFormData
} from "../../client"

const API = "/api/AgentReport"

export const useAgentDividendList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, `${API}/dividends`) : ""
    return useGet(url)
}

export const useAgentDividendDetailList = (searchParams) => {
    let url = searchParams
        ? urlWith(searchParams, `${API}/dividendDetails`)
        : ""
    return useGet(url)
}

export const useAgentWinLossList = (id, searchParams) => {
    let url =
        id && searchParams ? urlWith(searchParams, `${API}/${id}/winLoss`) : ""
    return useGet(url)
}
