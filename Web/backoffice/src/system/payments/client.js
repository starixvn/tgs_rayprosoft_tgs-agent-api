import useSWR, { cache } from "swr"
import { urlWith, useGet, useCreatePost, useUpdatePut } from "../../client"

const PaymentAPI = "/api/payment"

const PaymentChannelAPI = "/api/paymentchannel"

const PaymentMethodAPI = "/api/paymentmethod"

const PaymentVendorAPI = "/api/paymentvendor"

export const usePaymentList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, PaymentAPI) : ""
  return useGet(url)
}

export const usePayment = (id) => {
  let url = id ? `${PaymentAPI}/${id}` : ""
  return useGet(url)
}

export const useAddPayment = (data) => {
  return useCreatePost(PaymentAPI, data)
}

export const useUpdatePayment = (id, data) => {
  let url = id ? `${PaymentAPI}/${id}` : ""
  return useUpdatePut(url, data)
}

export const useEnablePayment = (id) => {
  let url = id ? `${PaymentAPI}/${id}/enable` : ""
  return useUpdatePut(url, {})
}

export const useDisablePayment = (id) => {
  let url = id ? `${PaymentAPI}/${id}/disable` : ""
  return useUpdatePut(url, {})
}

export const usePaymentHandingFee = (id) => {
  let url = id ? `${PaymentAPI}/${id}/handingFee` : ""
  return useGet(url)
}

export const useUpdatePaymentHandingFee = (id, data) => {
  let url = id ? `${PaymentAPI}/${id}/handingFee` : ""
  return useUpdatePut(url, data)
}

export const useChangePaymentOrders = (data) => {
  let url = `${PaymentAPI}/ChangeOrders`
  return useUpdatePut(url, data)
}

export const usePaymentChannelList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, PaymentChannelAPI) : ""
  return useGet(url)
}

export const usePaymentChannel = (id) => {
  let url = id ? `${PaymentChannelAPI}/${id}` : ""
  return useGet(url)
}

export const useAddPaymentChannel = (data) => {
  return useCreatePost(PaymentChannelAPI, data)
}

export const useUpdatePaymentChannel = (id, data) => {
  let url = id ? `${PaymentChannelAPI}/${id}` : ""
  return useUpdatePut(url, data)
}

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
