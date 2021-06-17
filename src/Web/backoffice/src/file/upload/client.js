import useSWR, { cache } from "swr"
import {
    urlWith,
    useGet,
    usePostData,
    usePut,
    usePutData,
    usePostFormData
} from "../../client"
import { FileUploadPath } from "./FileUploadPath"

export const useUploadImage = (body) => {
    let url = body ? FileUploadPath : undefined
    return usePostFormData(url, body)
}
