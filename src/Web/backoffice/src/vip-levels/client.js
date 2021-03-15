import useSWR from "swr"
import { urlWith, useGet, usePostData, usePutData } from "../client"

const API = "/api/vipLevel"

export const useVipLevelList = (searchParams) => {
    let url = urlWith(searchParams, API)
    let { data, ...rest } = useGet(url)
    if (data?.items?.length > 0) {
        data.items = data.items.map((item) => ({
            name: `VIP ${item.level}`,
            ...item
        }))
    }
    return {
        data,
        ...rest
    }
}

export const useAddVipLevel = (data) => {
    let url = data ? API : ""
    return usePostData(url, data)
}

export const useUpdateVipLevel = (id, data) => {
    let url = id ? `${API}/${id}` : ""
    return usePutData(url, data)
}
