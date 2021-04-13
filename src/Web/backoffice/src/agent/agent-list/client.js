import {
    urlWith,
    useGet,
    usePostData,
    usePutData,
    useDelete
} from "../../client"

let API = "/api/agent"

export const useAgentList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const useAgent = (id) => {
    let url = id ? `${API}/${id}` : ""
    let { data, ...rest } = useGet(url)
    return {
        data: data?.agent,
        ...rest
    }
}

export const useAddAgent = (formData) => {
    let url = formData ? API : ""
    return usePostData(url, formData)
}

export const useUpdateAgent = (agentId, data) => {
    let url = agentId ? `${API}/${agentId}` : ""
    return usePutData(url, data)
}

export const useAddAgentBank = (agentId, data) => {
    let url = agentId ? `${API}/${agentId}/bank` : ""
    return usePostData(url, data)
}

export const useUpdateAgentBank = (id, data) => {
    let url = id ? `${API}/bank/${id}` : ""
    return usePutData(url, data)
}

export const useDeleteAgentBank = (id) => {
    let url = id ? `${API}/bank/${id}` : ""
    return useDelete(url)
}

export const useAgentOperations = (agentId, searchParams) => {
    let url = agentId
        ? urlWith(searchParams, `${API}/${agentId}/operations`)
        : ""
    return useGet(url)
}
