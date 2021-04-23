import {
    urlWith,
    useGet,
    usePostData,
    usePutData,
    useDelete
} from "../../client"
import camelCase from "camelcase"
import camelcaseKeys from "camelcase-keys"

const API = "/api/setting"

export const useSystemSettingList = (searchParams) => {
    let url = searchParams ? urlWith(searchParams, API) : ""
    return useGet(url)
}

export const useSystemSetting = (id) => {
    let url = id ? `${API}/${id}` : ""
    return useGet(url)
}

export const useSystemSettingValue = (id) => {
    let { data, error, ...rest } = useSystemSetting(id)

    if (data && typeof data?.setting?.value === "string") {
        try {
            data = camelcaseKeys(JSON.parse(data?.setting?.value), {
                deep: true
            })
        } catch (error) {
            error = error ?? {
                title: "Syntax Error",
                message: error.message
            }
        }
    }

    return {
        data,
        error,
        ...rest
    }
}

export const useAddSystemSetting = (data) => {
    let url = data ? API : ""
    return usePostData(url, data)
}

export const useUpdateSystemSetting = (key, data) => {
    let url = key && data ? `${API}/${key}` : ""
    return usePutData(url, data)
}

export const useDeleteSystemSetting = (key) => {
    let url = key ? `/api/${key}` : ""
    return useDelete(url)
}
