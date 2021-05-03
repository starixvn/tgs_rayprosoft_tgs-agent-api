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

export const usePlayerAdjustBalance = (id, formData) => {
    let url = id && formData ? `${API}/${id}/adjust` : ""
    return usePostData(url, formData)
}
