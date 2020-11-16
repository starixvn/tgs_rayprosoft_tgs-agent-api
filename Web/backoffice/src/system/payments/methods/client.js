import useSWR, { cache } from "swr"
import { urlWith, useGet, useCreatePost, useUpdatePut } from "../../../client"

const PaymentMethodAPI = "/api/paymentmethod"

export const usePaymentMethodList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, PaymentMethodAPI) : ""
  return useGet(url)
}

export const usePaymentMethod = (id) => {
  let url = id ? `${PaymentMethodAPI}/${id}` : ""
  return useGet(url)
}

export const useAddPaymentMethod = (data) => {
  return useCreatePost(PaymentMethodAPI, data)
}

export const useUpdatePaymentMethod = (id, data) => {
  let url = id ? `${PaymentMethodAPI}/${id}` : ""
  return useUpdatePut(url, data)
}
