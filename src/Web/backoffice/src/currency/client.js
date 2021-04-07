import useSWR, { cache } from "swr"
import {
    urlWith,
    useGet,
    usePostData,
    usePut,
    usePutData,
    usePostFormData
} from "../client"

const API = "/api/currency"

export const useCurrencyList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const useAddCurrency = (data) => {
    return usePostData(API, data)
}

export const useUpdateCurrency = (code, data) => {
    let url = code ? `${API}/${code}` : ""
    return usePutData(url, data)
}
