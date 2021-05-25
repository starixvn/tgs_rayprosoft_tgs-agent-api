import useSWR, { cache } from "swr"
import {
    urlWith,
    useGet,
    usePostData,
    usePut,
    usePutData,
    usePostFormData
} from "../../client"

const API = "/api/Game"
export const useGameProviderGameList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const useAddGameProviderGameList = (data) => {
    let url = data ? API : ""
    return usePostData(url, data)
}

export const useUpdateGameProviderGame = (id, data) => {
    let url = id ? `${API}/${id}` : ""
    return usePutData(url, data)
}

export const useUploadGameProviderGames = (body) => {
    let url = body ? `${API}/upload` : undefined
    return usePostFormData(url, body)
}
