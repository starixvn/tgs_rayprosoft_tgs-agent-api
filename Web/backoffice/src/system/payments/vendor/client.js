import useSWR, { cache } from "swr"
import { urlWith, useGet, useCreatePost, useUpdatePut } from "../../../client"

const PaymentVendorAPI = "/api/paymentvendor"

export const usePaymentVendorList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, PaymentVendorAPI) : ""
  return useGet(url)
}

export const usePaymentVendor = (id) => {
  let url = id ? `${PaymentVendorAPI}/${id}` : ""
  return useGet(url)
}

export const useAddPaymentVendor = (data) => {
  return useCreatePost(PaymentVendorAPI, data)
}

export const useUpdatePaymentVendor = (id, data) => {
  let url = id ? `${PaymentVendorAPI}/${id}` : ""
  return useUpdatePut(url, data)
}
