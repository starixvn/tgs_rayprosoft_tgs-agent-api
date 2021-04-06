import useSWR, { cache } from "swr"
import {
    urlWith,
    useGet,
    usePostData,
    usePut,
    usePutData,
    usePostFormData
} from "../../client"

const API = "/api/RoundReport"

export const useGameRoundList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const useGameRound = (id, searchParams) => {
    let url =
        id &&
        searchParams?.gameProviderCode &&
        searchParams?.playerCode &&
        searchParams?.language
            ? urlWith(searchParams, `${API}/${id}`)
            : ""

    return useGet(url)
}
