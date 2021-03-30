import useSWR, { cache } from "swr"
import {
    urlWith,
    useGet,
    usePostData,
    usePut,
    usePutData,
    usePostFormData
} from "../../client"

const API = "/api/agentContribution"

export const useAgentContributionList = (searchParams) => {
    //TODO: rm test
    // return useGet(searchParams ? urlWith(searchParams, "/api/vipReport") : "")

    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}
