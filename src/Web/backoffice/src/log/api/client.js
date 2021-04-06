import {
    urlWith,
    useGet,
    usePostData,
    usePutData,
    useDelete
} from "../../client"

let API = "/api/log/api"

export const useApiLogList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const useApiLog = (id) => {
    let url = id ? `${API}/${id}` : ""
    let { data, ...rest } = useGet(url)
    return {
        data: data?.logDetail,
        ...rest
    }
}
