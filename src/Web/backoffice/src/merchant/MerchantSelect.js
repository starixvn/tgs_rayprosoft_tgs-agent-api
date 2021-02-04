import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import {
  Spinner,
  Stack,
  Flex,
  Box,
  Card,
  Button,
  Text,
  Input,
  Checkbox,
  FieldStack,
  FieldWrapper,
  Select,
  SelectField,
  Table,
  SwitchField,
  Group,
  Set
} from "bumbag"
import { Reseller } from "./resellers"
import { Licensee } from "./licensees"
import { Brand } from "./brands"
import { useResellerList } from "./resellers/client"
import { useLicenseeList } from "./licensees/client"
import { useBrandList } from "./brands/client"
import { useTrans } from "../i18n"
import { useFormContext } from "react-hook-form"

const isActive = (merchant, start, end) => {
  const order = [Reseller, Licensee, Brand]
  let merchantIndex = order.findIndex((item) => item === merchant)
  let startIndex = order.findIndex((item) => item === start)
  let endIndex = order.findIndex((item) => item === end)
  return (
    merchantIndex >= 0 &&
    merchantIndex >= startIndex &&
    merchantIndex <= endIndex
  )
}

// null or "" -> undefined
const normalize = (data) => (!data ? undefined : data)

const useResellerPlaceholder = () =>
  useTrans({
    zh: "选择总公司",
    en: "Select Reseller"
  })

const useLicenseePlaceholder = () =>
  useTrans({
    zh: "选择公司",
    en: "Select Licensee"
  })

const useBrandPlaceholder = () =>
  useTrans({
    zh: "选择品牌",
    en: "Select Brand"
  })

export const MerchantIdSelect = ({
  start = Reseller,
  end = Brand,

  resellerId: initialResellerId = "",
  resellerIdName = "resellerId",

  licenseeId: initialLicenseeId = "",
  licenseeIdName = "licenseeId",

  brandId: initialBrandId = "",
  brandIdName = "brandId",
  onBrandChange,

  required = true,
  ...props
}) => {
  let { register, setValue, getValues, errors, watch } = useFormContext()

  let isResellerActive = isActive(Reseller, start, end)
  let resellerId = watch(resellerIdName)
  let resellerError = errors[resellerIdName]
  let {
    data: resellerData,
    isLoading: isResellerDataLoading
  } = useResellerList(isResellerActive ? { showAll: true } : undefined)
  let resellerOptions =
    resellerData?.items.map((reseller) => ({
      label: reseller.code,
      value: reseller.id
    })) ?? []

  let isLicenseeActive = isActive(Licensee, start, end)
  let licenseeId = watch(licenseeIdName)
  let licenseeError = errors[licenseeIdName]
  let {
    data: licenseeData,
    isLoading: isLicenseeDataLoading
  } = useLicenseeList(
    isLicenseeActive ? { showAll: true, resellerId } : undefined
  )
  let licenseeOptions =
    licenseeData?.items.map((licensee) => ({
      label: licensee.code,
      value: licensee.id
    })) ?? []

  let isBrandActive = isActive(Brand, start, end)
  let brandId = watch(brandIdName)
  let brandError = errors[brandIdName]
  let { data: brandData, isLoading: isBrandDataLoading } = useBrandList(
    isBrandActive ? { showAll: true, licenseeId } : undefined
  )
  let brandOptions =
    brandData?.items.map((brand) => ({
      label: brand.code,
      value: brand.id
    })) ?? []

  useEffect(() => {
    onBrandChange?.(brandId)
  }, [brandId])

  let resellerPlaceholder = useResellerPlaceholder()
  let licenseePlaceholder = useLicenseePlaceholder()
  let brandPlaceHolder = useBrandPlaceholder()
  return (
    <Group>
      {isResellerActive && (
        <Select
          name={resellerIdName}
          required={required}
          selectRef={register({ required })}
          state={resellerError && "danger"}
          isLoading={isResellerDataLoading}
          options={resellerOptions}
          defaultValue={resellerId}
          placeholder={resellerPlaceholder}
          onChange={(event) => {
            setValue(licenseeIdName, undefined)
            setValue(brandIdName, undefined)
          }}
        />
      )}
      {isLicenseeActive && (
        <Select
          name={licenseeIdName}
          required={required}
          selectRef={register({ required })}
          state={licenseeError && "danger"}
          isLoading={isLicenseeDataLoading}
          options={licenseeOptions}
          defaultValue={licenseeId}
          placeholder={licenseePlaceholder}
          onChange={(event) => {
            setValue(brandIdName, undefined)
          }}
        />
      )}
      {isBrandActive && (
        <Select
          name={brandIdName}
          required={required}
          selectRef={register({ required })}
          state={brandError && "danger"}
          isLoading={isBrandDataLoading}
          options={brandOptions}
          defaultValue={brandId}
          placeholder={brandPlaceHolder}
        />
      )}
    </Group>
  )
}
