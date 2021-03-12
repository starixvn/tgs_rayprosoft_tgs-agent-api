import {
    urlWith,
    useGet,
    usePostData,
    usePutData,
    useDelete
} from "../../client"

const API = "/api/setting"

export const useSystemSettingList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const useSystemSetting = (id) => {
    let url = id ? `${API}/${id}` : ""
    return useGet(url)
}

export const useAddSystemSetting = (data) => {
    let url = data ? API : ""
    return usePostData(url, data)
}

export const useUpdateSystemSetting = (id, data) => {
    let url = id ? `${API}/${id}` : ""
    return usePutData(url, data)
}

export const useDeleteSystemSetting = (id) => {
    let url = id ? `/api/role/${id}` : ""
    return useDelete(url)
}
