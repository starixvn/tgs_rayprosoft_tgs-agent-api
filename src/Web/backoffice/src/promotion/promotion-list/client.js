import {
    urlWith,
    useGet,
    usePostData,
    usePut,
    usePutData,
    useDelete
} from "../../client"
import {
    useSystemSetting,
    useSystemSettingValue
} from "../../system-admin/setting/client"

let API = "/api/promotion"

export const usePromotionList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const usePromotion = (id) => {
    let url = id ? urlWith(searchParams, `${API}/${id}`) : ""
    return useGet(url)
}

export const useAddPromotion = (data) => {
    return usePostData(API, data)
}

export const useUpdatePromotion = (id, data) => {
    let url = id ? `${API}/${id}` : ""
    return usePutData(url, data)
}

export const useChangePromotionStatus = (id, status) => {
    let url = id && status ? `${API}/${id}/${status}` : ""

    return usePut(url)
}
