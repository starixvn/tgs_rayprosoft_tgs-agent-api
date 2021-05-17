import useSWR, { cache } from "swr"
import {
    urlWith,
    useGet,
    usePostData,
    usePut,
    usePutData,
    usePostFormData
} from "../../client"

const API = "/api/AgentReport"

export const useAgentContributionList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, `${API}/contributions`) : ""
    return useGet(url)
}

export const useAgentTeamInfo = (agentId, searchParams) => {
    let url = agentId ? urlWith(searchParams, `${API}/${agentId}/teamInfo`) : ""
    return useGet(url)
}
