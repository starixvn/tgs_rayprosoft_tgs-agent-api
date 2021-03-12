import useSWR from "swr"
import { urlWith, useGet, usePostData, usePutData } from "../client"

const API = "/api/vipLevel"

export const useVipLevelList = (searchParams) => {
    let url = urlWith(searchParams, API)
    return useGet(url)
}

export const useAddVipLevel = (data) => {
    let url = data ? API : ""
    return usePostData(url, data)
}

export const useUpdateVipLevel = (id, data) => {
    let url = id ? `${API}/${id}` : ""
    return usePutData(url, data)
}
