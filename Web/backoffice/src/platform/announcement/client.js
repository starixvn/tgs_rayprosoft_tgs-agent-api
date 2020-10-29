import useSWR, { cache } from "swr"
import client, { urlWith, get, put, post } from "../../client"

let API = "/api/announcement"

export const useList = (searchParams = {}) => {
  let key = urlWith(searchParams, API)
  const { data, error, isValidating, mutate } = useSWR(key, get)

  return {
    data,
    isLoading: !error && !data,
    isValidating,
    error,
    isError: error,
    mutate
  }
}

export const useVerifyAnnouncement = ({ id, type, data }) => {
  let fetchData = !!id && !!type && !!data
  let keys = fetchData ? [`${API}/${id}/${type}`, data] : null
  let { data: result, error, isValidating, mutate } = useSWR(
    keys,
    (url, data) => put(url, { data })
  )

  return {
    result,
    isLoading: !error && !result && fetchData,
    isValidating,
    error,
    isError: error,
    mutate
  }
}

export const useAddAnnouncement = ({ data }) => {
  let fetchData = !!data
  let keys = fetchData ? [API, data] : null
  let { data: result, error, isValidating, mutate } = useSWR(
    keys,
    (url, data) => post(url, { data })
  )

  return {
    result,
    isLoading: !error && !result && fetchData,
    isValidating,
    error,
    isError: error,
    mutate
  }
}

export const useEditAnnouncement = ({ id, data }) => {
  let fetchData = id && !!data
  let keys = fetchData ? [`${API}/${id}`, data] : null
  let { data: result, error, isValidating, mutate } = useSWR(
    keys,
    (url, data) => put(url, { data })
  )

  return {
    result,
    isLoading: !error && !result && fetchData,
    isValidating,
    error,
    isError: error,
    mutate
  }
}
