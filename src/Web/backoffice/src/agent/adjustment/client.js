import {
    urlWith,
    useGet,
    usePostData,
    usePutData,
    useDelete
} from "../../client"

let API = "/api/agent/adjustment"

export const useAgentAdjustmentList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const useAgentAddBalance = ({ brandId, username }, formData) => {
    let url = brandId && username && formData ? `${API}/add` : ""
    return usePostData(url, formData)
}

export const useAgentDeductBalance = ({ brandId, username }, formData) => {
    let url = brandId && username && formData ? `${API}/deduct` : ""
    return usePostData(url, formData)
}
