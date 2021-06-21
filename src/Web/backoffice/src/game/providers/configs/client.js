import useSWR, { cache } from "swr"
import {
    urlWith,
    useGet,
    usePostData,
    usePut,
    usePutData,
    usePostFormData
} from "../../../client"

const API = "/api/GameProviderConfiguration"

export const useGameProviderConfigList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const useGameProviderConfig = (id) => {
    let url = id ? `${API}/${id}` : undefined
    let { data, ...rest } = useGet(url)
    return {
        data: data?.gameProviderConfiguration,
        ...rest
    }
}

export const useAddGameProviderConfig = (data) => {
    return usePostData(API, data)
}

export const useUpdateGameProviderConfig = (id, data) => {
    let url = id ? `${API}/${id}` : ""
    return usePutData(url, data)
}
