import {
    urlWith,
    useGet,
    usePostData,
    usePutData,
    useDelete
} from "../../client"

let API = "/api/deposit"

export const useFundDepositFormList = (brandId, searchParams, options) => {
    let url =
        brandId && searchParams
            ? urlWith(searchParams, `${API}/${brandId}`)
            : ""
    return useGet(url, options)
}

export const useFundDepositForm = (brandId, formId, searchParams) => {
    let url =
        brandId && formId
            ? urlWith(searchParams, `${API}/${brandId}/${formId}`)
            : ""
    let { data, ...rest } = useGet(url)

    return {
        data: data?.data,
        ...rest
    }
}

export const useVerifyFundDepositForm = (brandId, formId, formData) => {
    let url = brandId && formId && formData ? `${API}/${formId}` : ""

    return usePostData(url, formData)
}
