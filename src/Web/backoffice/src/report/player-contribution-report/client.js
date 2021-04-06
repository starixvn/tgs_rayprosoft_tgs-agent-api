import useSWR, { cache } from "swr"
import {
    urlWith,
    useGet,
    usePostData,
    usePut,
    usePutData,
    usePostFormData
} from "../../client"

const API = "/api/PlayerReport"

export const usePlayerContributionList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, `${API}/contribution`) : ""
    return useGet(url)
}
