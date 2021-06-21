import useSWR, { cache } from "swr"
import {
    urlWith,
    useGet,
    usePostData,
    usePut,
    usePutData,
    usePostFormData
} from "../../client"

const API = "/api/fs"

export const useUploadImage = (prefix, body) => {
    let url = prefix && body ? `${API}/upload/${prefix}` : undefined
    return usePostFormData(url, body)
}
