import useSWR from "swr"
import client, { urlWith, get, put, post } from "../../client"

export const useList = (searchParams) => {
  let url = urlWith(searchParams, `/api/department`)
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

export const useAddDepartment = ({ data }) => {
  let fetchData = !!data
  const { data: result, error, isValidating, mutate } = useSWR(
    fetchData ? [`/api/department/`, data] : null,
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

export const useEditDepartment = ({ id, data }) => {
  let fetchData = id && !!data
  const { data: result, error, isValidating, mutate } = useSWR(
    fetchData ? [`/api/department/${id}`, data] : null,
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
