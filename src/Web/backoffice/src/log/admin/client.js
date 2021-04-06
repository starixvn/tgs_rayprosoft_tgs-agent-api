import {
    urlWith,
    useGet,
    usePostData,
    usePutData,
    useDelete
} from "../../client"

let API = "/api/log/admin"

export const useAdminLogList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const useAdminLog = (id) => {
    let url = id ? `${API}/${id}` : ""
    let { data, ...rest } = useGet(url)
    return {
        data: data?.logDetail,
        ...rest
    }
}
