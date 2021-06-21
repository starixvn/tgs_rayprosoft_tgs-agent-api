import { toDataOrError } from "./response"
import useSWR, { cache } from "swr"
import { getAcceptLang, getAcceptLangCulture } from "../i18n"

export const urlWith = (searchParams = {}, url) => {
    let urlSearchParams = Object.entries(searchParams).reduce(
        (searchParams, [key, value]) => {
            if (isEmptyString(value) || isNilOrUndefined(value)) {
                return searchParams
            }

            if (Array.isArray(value) && value.length > 0) {
                let result = value.reduce((result, item) => {
                    return result
                        ? `${result}&${key}=${stringOf(item)}`
                        : `${key}=${stringOf(item)}`
                }, "")
                return searchParams ? `${searchParams}&${result}` : result
            }

            return searchParams
                ? `${searchParams}&${key}=${stringOf(value)}`
                : `${key}=${stringOf(value)}`
        },
        ""
    )
    return urlSearchParams ? `${url}?${urlSearchParams}` : url
}

const stringOf = (value) => value?.toISOString?.() ?? value

const FailedToFetchDefaultMessage = "Failed to fetch"
const FailedToFetchTrans = {
    zh: "获取数据失败",
    en: FailedToFetchDefaultMessage
}
const reject = (error) => {
    let locale = getAcceptLang()
    let title = error?.message ?? error?.toString?.() ?? "Error"
    if (title == "Failed to fetch") {
        title = FailedToFetchTrans?.[locale] ?? FailedToFetchDefaultMessage
    } else {
        console.warn(error)
    }
    return Promise.reject({ title })
}

export const get = (
    url = "",
    { searchParams = {}, headers = {}, ...options } = {}
) => {
    url = urlWith(searchParams, url)
    if (!headers["Accept-Language"]) {
        headers["Accept-Language"] = getAcceptLangCulture()
    }

    return fetch(url, { headers, ...options })
        .catch((error) => {
            console.warn(error)
            return reject(error)
        })
        .then(toDataOrError)
}

export const put = (url, options) => fetchApi("PUT", url, options)

export const post = (url, options) => fetchApi("POST", url, options)

const fetchApi = (
    method = "",
    url,
    { headers = {}, body, data, ...options } = {}
) => {
    if (!headers["Accept-Language"]) {
        headers["Accept-Language"] = getAcceptLangCulture()
    }

    headers = data
        ? {
              "Content-Type": "application/json",
              ...headers
          }
        : headers

    return fetch(url, {
        method,
        headers,
        body: body ?? JSON.stringify(dropEmptyValues(data)),
        ...options
    })
        .catch((error) => {
            console.warn(error)
            return reject(error)
        })
        .then(toDataOrError)
}

const dropEmptyValues = (record) => {
    if (record === null || typeof record != "object") {
        //Note. typeof null = "object"
        return record
    }

    return Object.entries(record).reduce((data, [key, value]) => {
        if (isEmptyString(value) || isNilOrUndefined(value)) {
            return data
        }

        data[key] = value
        return data
    }, {})
}

const DefaultUpdateOptions = {
    dedupingInterval: 100
}

const deleteCacheBy = (key = "") => {
    if (key) {
        cache.delete(key)
        cache.delete(`err@${key}`)
        cache.delete(`req@${key}`)
    }
}

export const useGet = (url, options = DefaultUpdateOptions) => {
    let fetchData = !!url
    let key = fetchData ? url : null
    const { data, error, isValidating, mutate } = useSWR(key, get, options)

    return {
        data,
        isLoading: !error && !data && fetchData,
        isValidating,
        error,
        isError: error,
        mutate,
        deleteCache: () => deleteCacheBy(key),
        clearCache: () => cache.clear()
    }
}

export const usePostData = (url, data, options = DefaultUpdateOptions) => {
    let fetchData = !!url && data !== undefined
    let keys = fetchData ? [url, data] : null
    let {
        data: result,
        error,
        isValidating,
        mutate
    } = useSWR(keys, (url, data) => post(url, { data }), options)

    return {
        result,
        isLoading: !error && !result && fetchData,
        isValidating,
        error,
        isError: error,
        mutate,
        deleteCache: () => deleteCacheBy(keys),
        clearCache: () => cache.clear()
    }
}

export const usePostFormData = (
    url,
    formData,
    options = DefaultUpdateOptions
) => {
    let fetchData = !!url && !!formData
    let keys = fetchData ? [url, formData] : null
    let {
        data: result,
        error,
        isValidating,
        mutate
    } = useSWR(
        keys,
        (url, formData) =>
            post(url, {
                body: formData
            }),
        options
    )

    return {
        result,
        isLoading: !error && !result && fetchData,
        isValidating,
        error,
        isError: error,
        mutate,
        deleteCache: () => deleteCacheBy(keys),
        clearCache: () => cache.clear()
    }
}

export const usePutData = (url, data, options = DefaultUpdateOptions) => {
    let fetchData = !!url && !!data
    let keys = fetchData ? [url, data] : null
    let {
        data: result,
        error,
        isValidating,
        mutate
    } = useSWR(keys, (url, data) => put(url, { data }), options)

    return {
        result,
        isLoading: !error && !result && fetchData,
        isValidating,
        error,
        isError: error,
        mutate,
        deleteCache: () => deleteCacheBy(keys),
        clearCache: () => cache.clear()
    }
}

export const usePut = (url, settings, options = DefaultUpdateOptions) => {
    let keys = url ? [url, settings] : null
    let {
        data: result,
        error,
        isValidating,
        mutate
    } = useSWR(keys, put, options)

    return {
        result,
        isLoading: keys && !error && !result,
        isValidating,
        error,
        isError: error,
        mutate,
        deleteCache: () => deleteCacheBy(keys),
        clearCache: () => cache.clear()
    }
}

export const useDelete = (url, settings) => {
    let fetchData = !!url
    let key = fetchData ? url : null
    const {
        data: result,
        error,
        isValidating,
        mutate
    } = useSWR(key, (url) => fetchApi("DELETE", url, settings))

    return {
        result,
        isLoading: !error && !result && fetchData,
        isValidating,
        error,
        isError: error,
        mutate,
        deleteCache: () => deleteCacheBy(key),
        clearCache: () => cache.clear()
    }
}

const isEmptyString = (value) => typeof value == "string" && value === ""

const isNilOrUndefined = (value) =>
    typeof value === "undefined" || value === null

export default {
    get,
    put,
    post,
    delete: (url, options) => fetchApi("DELETE", url, options)
}
