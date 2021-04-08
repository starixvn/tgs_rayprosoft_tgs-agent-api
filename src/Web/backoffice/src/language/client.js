import useSWR, { cache } from "swr"
import {
    urlWith,
    useGet,
    usePostData,
    usePut,
    usePutData,
    usePostFormData
} from "../client"

const API = "/api/language"

export const useLanguageList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const useAddLanguage = (data) => {
    return usePostData(API, data)
}

export const useUpdateLanguage = (code, data) => {
    let url = code ? `${API}/${code}` : ""
    return usePutData(url, data)
}
