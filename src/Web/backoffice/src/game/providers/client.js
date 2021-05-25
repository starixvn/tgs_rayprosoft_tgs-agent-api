import useSWR, { cache } from "swr"
import {
    urlWith,
    useGet,
    usePostData,
    usePut,
    usePutData,
    usePostFormData
} from "../../client"

const API = "/api/GameProvider"

export const useGameProviderList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const useGameProvider = (id) => {
    let url = id ? `${API}/${id}` : undefined
    let { data, ...rest } = useGet(url)
    return {
        data: data?.gameProvider,
        ...rest
    }
}

export const useAddGameProvider = (data) => {
    return usePostData(API, data)
}

export const useAddGameProviderGame = (gameProviderId, data) => {
    console.warn("useAddGameProviderGame not impl yet")
    return usePostData(API, data)
}

export const useAddGameProviderGameList = (gameProviderId, data) => {
    console.warn("useAddGameProviderGameList not impl yet")
    return {
        isLoading: false,
        result: data ? {} : undefined,
        error: undefined,
        mutate: () => {
            console.log("TODO: mutate")
        },
        deleteCache: () => {
            console.log("TODO: deleteCache")
        }
    }
    // return usePostData(API, data)
}

export const useUpdateGameProvider = (id, data) => {
    let url = id ? `${API}/${id}` : ""
    return usePutData(url, data)
}

export const useUpdateGameProviderGame = (id, data) => {
    let url = id ? `${API}/${id}` : ""
    return usePutData(url, data)
}

export const useChangeGameProviderStatus = (id, status) => {
    let url = id && status ? `${API}/${id}/${status}` : ""
    return usePut(url)
}

export const useUploadGameProviderGames = (body) => {
    let url = body ? `${API}/upload` : undefined
    return usePostFormData(url, body)
}
