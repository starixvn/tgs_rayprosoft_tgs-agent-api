import useSWR, { cache } from "swr"
import {
    urlWith,
    useGet,
    usePostData,
    usePut,
    usePutData,
    usePostFormData
} from "../../client"

const API = "/api/vipReport"

export const useVipReportList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const usePlayerVipRebateList = (username, searchParams) => {
    let url =
        username && searchParams
            ? urlWith(searchParams, `${API}/${username}`)
            : ""
    return useGet(url)
}
