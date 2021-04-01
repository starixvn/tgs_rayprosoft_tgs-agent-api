import {
    urlWith,
    useGet,
    usePostData,
    usePutData,
    useDelete
} from "../../client"

let API = "/api/player/adjustment"

export const usePlayerAdjustmentList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const usePlayerAddBalance = ({ brandId, username }, formData) => {
    let url = brandId && username && formData ? `${API}/add` : ""
    return usePostData(url, formData)
}

export const usePlayerDeductBalance = ({ brandId, username }, formData) => {
    let url = brandId && username && formData ? `${API}/deduct` : ""
    return usePostData(url, formData)
}
