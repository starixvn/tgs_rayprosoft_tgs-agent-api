import useSWR, { cache } from "swr"
import {
  urlWith,
  useGet,
  usePostData,
  usePut,
  usePutData,
  usePostFormData
} from "../../client"
//TODO
const API = ""

export const useProviderList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  return useGet(url)
}

export const useAddProvider = (data) => {
  return usePostData(API, data)
}

export const useUpdateProvider = (id, data) => {
  let url = id ? `${API}/${id}` : ""
  return usePutData(url, data)
}

export const useUpdatePresetMaintenance = (id, data) => {
  let url = id ? `${API}/${id}/TODO` : ""
  return usePutData(url, data)
}

export const useChangeProviderStatus = (id, status) => {
  let url = id && status ? `${API}/${id}/${status}` : ""
  return usePut(url)
}
