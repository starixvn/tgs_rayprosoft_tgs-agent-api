import useSWR, { cache } from "swr"
import {
  urlWith,
  useGet,
  usePostData,
  usePut,
  usePutData,
  usePostFormData
} from "../../client"

const API = "/api/reseller"

export const useResellerList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  return useGet(url)
}

export const useReseller = (id) => {
  let url = id ? `${API}/${id}` : ""
  let { data, ...rest } = useGet(url)

  return {
    data: data?.reseller,
    ...rest
  }
}

export const useAddReseller = (data) => {
  let url = data ? API : ""
  return usePostData(url, data)
}

export const useUpdateReseller = (id, data) => {
  let url = id ? `${API}/${id}` : ""
  return usePutData(url, data)
}

export const useChangeResellerStatus = (id, status) => {
  let url = id && status ? `${API}/${id}/${status}` : ""
  return usePut(url)
}
