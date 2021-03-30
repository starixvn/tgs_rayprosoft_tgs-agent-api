import useSWR, { cache } from "swr"
import {
    urlWith,
    useGet,
    usePostData,
    usePut,
    usePutData,
    usePostFormData
} from "../../client"

const API = "/api/agentbonus"

export const useAgentBonusList = (username, searchParams) => {
    //TODO: rm test code
    // return useGet(searchParams ? urlWith(searchParams, "/api/vipReport") : "")

    let url =
        username && searchParams
            ? urlWith(searchParams, `${API}/${username}`)
            : ""
    return useGet(url)
}

export const useAgentWinLossList = (username, searchParams) => {
    //TODO: rm test code
    // return useGet(searchParams ? urlWith(searchParams, "/api/vipReport") : "")

    let url =
        username && searchParams
            ? urlWith(searchParams, `${API}/${username}/winLoss`)
            : ""
    return useGet(url)
}

export const useAgentBonusDetailList = (username, searchParams) => {
    //TODO: rm test code
    // return useGet(searchParams ? urlWith(searchParams, "/api/vipReport") : "")

    let url =
        username && searchParams
            ? urlWith(searchParams, `${API}/${username}/bonus`)
            : ""
    return useGet(url)
}
