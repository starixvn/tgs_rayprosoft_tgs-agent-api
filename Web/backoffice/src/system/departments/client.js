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
