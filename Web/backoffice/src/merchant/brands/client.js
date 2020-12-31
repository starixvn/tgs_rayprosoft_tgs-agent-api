import useSWR, { cache } from "swr"
import {
  urlWith,
  useGet,
  usePostData,
  usePut,
  usePutData,
  usePostFormData
} from "../../client"

const API = "/api/brand"

export const useBrandList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  return useGet(url)
}

export const useBrand = (id) => {
  let url = id ? `${API}/${id}` : ""
  return useGet(url)
}

export const useAddBrand = (data) => {
  let url = data ? API : ""
  return usePostData(url, data)
}

export const useUpdateBrand = (id, data) => {
  let url = id ? `${API}/${id}` : ""
  return usePutData(url, data)
}

export const useChangeBrandStatus = (id, status) => {
  let url = id && status ? `${API}/${id}/${status}` : ""
  return usePut(url)
}
