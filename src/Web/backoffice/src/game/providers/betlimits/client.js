import useSWR, { cache } from "swr"
import {
  urlWith,
  useGet,
  usePostData,
  usePut,
  usePutData,
  usePostFormData
} from "../../../client"

const API = "/api/GameProviderBetLimit"

export const useGameProviderBetLimitList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  let { data, ...rest } = useGet(url)
  return {
    items: data?.betLimits ?? [],
    ...rest
  }
}

export const useGameProviderBetLimit = (id) => {
  let url = id ? `${API}/${id}` : undefined
  return useGet(url)
}

export const useGameProviderBetLimitInUse = (id) => {
  let url = id ? `${API}/${id}/InUse` : undefined
  return useGet(url)
}

export const useAddGameProviderBetLimit = (data) => {
  return usePostData(API, data)
}

export const useUpdateGameProviderBetLimit = (id, data) => {
  let url = id ? `${API}/${id}` : ""
  return usePutData(url, data)
}

export const useChangeGameProviderBetLimitStatus = (id, status) => {
  let url = id && status ? `${API}/${id}/${status}` : ""
  return usePut(url)
}
