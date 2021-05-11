import useSWR, { cache } from "swr"
import {
    urlWith,
    useGet,
    usePostData,
    usePut,
    usePutData,
    usePostFormData
} from "../../client"

const API = "/api/brandpayment"

export const useBrandPaymentList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const useBrandPayment = (id, searchParams) => {
    let { data, ...rest } = useBrandPaymentList(searchParams)

    data = data?.items?.find((item) => item.id === id)

    return {
        data,
        ...rest
    }
    /*
    let url = id ? `${API}/${id}` : ""
    return useGet(url)
    */
}

export const useAddBrandPayment = (data) => {
    return usePostData(API, data)
}

export const useUpdatePayment = (id, data) => {
    let url = id ? `${API}/${id}` : ""
    return usePutData(url, data)
}

export const useBrandPaymentHandingFee = (id) => {
    let url = id ? `${API}/${id}/handingFee` : ""
    return useGet(url)
}

export const useUpdateBrandPaymentHandingFee = (id, data) => {
    let url = id ? `${API}/${id}/handingFee` : ""
    return usePutData(url, data)
}

export const useBrandPaymentUpload = (file) => {
    let url = file ? `${API}/upload` : ""
    return usePostFormData(url, file)
}

export const useChangeBrandPaymentOrders = (data) => {
    let url = `${API}/ChangeOrders`
    return usePutData(url, data)
}

export const useChangeBrandPaymentStatus = (id, status) => {
    let url = id && status ? `${API}/${id}/${status}` : ""
    return usePut(url)
}

export const useBrandPaymentLogs = (id) => {
    let url = id ? `${API}/${id}/auditLogs` : ""
    return useGet(url)
}

export const useBrandPaymentDesignatedUsers = (id) => {
    let url = id ? `${API}/${id}/designatedUsers` : ""
    return useGet(url)
}

export const useBrandPaymentMethodChannelList = () => {
    let { data, ...rest } = useGet(`${API}/pmc`)

    return {
        data: data?.data,
        ...rest
    }
}
