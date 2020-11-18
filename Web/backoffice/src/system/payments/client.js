import useSWR, { cache } from "swr"
import {
  urlWith,
  useGet,
  useCreatePost,
  useUpdatePut,
  usePostFormData
} from "../../client"

const API = "/api/payment"

export const usePaymentList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  return useGet(url)
}

export const usePayment = (id) => {
  let url = id ? `${API}/${id}` : ""
  return useGet(url)
}

export const useAddPayment = (data) => {
  return useCreatePost(API, data)
}

export const useUpdatePayment = (id, data) => {
  let url = id ? `${API}/${id}` : ""
  return useUpdatePut(url, data)
}

export const useEnablePayment = (id) => {
  let url = id ? `${API}/${id}/enable` : ""
  return useUpdatePut(url, {})
}

export const useDisablePayment = (id) => {
  let url = id ? `${API}/${id}/disable` : ""
  return useUpdatePut(url, {})
}

export const usePaymentHandingFee = (id) => {
  let url = id ? `${API}/${id}/handingFee` : ""
  return useGet(url)
}

export const useUpdatePaymentHandingFee = (id, data) => {
  let url = id ? `${API}/${id}/handingFee` : ""
  return useUpdatePut(url, data)
}

export const useUpload = (file) => {
  let url = file ? `${API}/upload` : ""
  return usePostFormData(url, file)
}

export const useChangePaymentOrders = (data) => {
  let url = `${API}/ChangeOrders`
  return useUpdatePut(url, data)
}

export const useChangePaymentStatus = (id, status) => {
  let url = id && status ? `${API}/${id}/${status}` : ""
  return useUpdatePut(url, status)
}
