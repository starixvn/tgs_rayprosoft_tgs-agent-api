import useSWR from "swr"
import client, { urlWith, get, put, post } from "../../client"

export const useList = (searchParams) => {
  let url = urlWith(searchParams, `/api/users`)
  const { data, error, isValidating, mutate } = useSWR(url, get)

  return {
    data,
    isLoading: !error && !data,
    isValidating,
    error,
    isError: error,
    mutate
  }
}

export const useAddAdmin = ({ data }) => {
  let fetchData = !!data
  const { data: result, error, isValidating, mutate } = useSWR(
    fetchData ? [`/api/users/`, data] : null,
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

export const useEditAdmin = ({ id, data }) => {
  let fetchData = id && data
  const { data: result, error, isValidating, mutate } = useSWR(
    fetchData ? [`/api/users/${id}`, data] : null,
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

export const useEditPassword = ({ id, data }) => {
  let fetchData = id && data
  const { data: result, error, isValidating, mutate } = useSWR(
    fetchData ? [`/api/users/${id}/password`, data] : null,
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
