import {
    urlWith,
    useGet,
    usePostData,
    usePutData,
    useDelete
} from "../../client"

let API = "/api/AgentAdjust"

export const useAgentAdjustmentList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const useAgentAdjustBalance = (agentId, formData) => {
    let url = agentId && formData ? `${API}/${agentId}/adjust` : ""
    return usePostData(url, formData)
}
