import useSWR from "swr"
import { useGet, usePostData, usePutData } from "../client"

const API = "/api/vipLevel"

export const useVipLevelList = () => {
    let { data, ...rest } = useGet(API)
    return {
        items: data?.vipLevels,
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
