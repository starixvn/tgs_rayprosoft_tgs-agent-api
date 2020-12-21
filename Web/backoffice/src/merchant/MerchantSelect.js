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
  SelectField,
  Table,
  Select,
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

//TODO: required validation?
export const MerchantIdSelect = ({
  start = Reseller,
  end = Brand,
  resellerIdName = "resellerId",
  resellerId: initialResellerId = "",
  licenseeIdName = "licenseeId",
  licenseeId: initialLicenseeId = "",
  brandIdName = "brandId",
  brandId: initialBrandId = "",
  range = [],
  onChange,
  ...props
}) => {
  let isResellerActive = isActive(Reseller, start, end)
  let [resellerId, setResellerId] = useState(initialResellerId)
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
  let [licenseeId, setLicenseeId] = useState(initialLicenseeId)
  let {
    data: licenseeData,
    isLoading: isLicenseeDataLoading
  } = useLicenseeList(isLicenseeActive ? { showAll: true } : undefined)
  let licenseeOptions =
    licenseeData?.items.map((licensee) => ({
      label: licensee.code,
      value: licensee.id
    })) ?? []

  let isBrandActive = isActive(Brand, start, end)
  let [brandId, setBrandId] = useState(initialBrandId)
  let { data: brandData, isLoading: isBrandDataLoading } = useBrandList(
    isBrandActive ? { showAll: true } : undefined
  )
  let brandOptions =
    brandData?.items.map((brand) => ({
      label: brand.code,
      value: brand.id
    })) ?? []

  useEffect(() => {
    onChange?.({
      resellerId,
      licenseeId,
      brandId
    })
  }, [resellerId, licenseeId, brandId])

  let resellerPlaceholder = useResellerPlaceholder()
  let licenseePlaceholder = useLicenseePlaceholder()
  let brandPlaceHolder = useBrandPlaceholder()
  return (
    <Group>
      {isResellerActive && (
        <Select
          name={resellerIdName}
          isLoading={isResellerDataLoading}
          value={resellerId}
          options={resellerOptions}
          onChange={(event) => {
            let value = normalize(event.target.value)
            setResellerId(value)

            //reset
            setLicenseeId(undefined)
            setBrandId(undefined)
          }}
          placeholder={resellerPlaceholder}
        />
      )}
      {isLicenseeActive && (
        <Select
          name={licenseeIdName}
          isLoading={isLicenseeDataLoading}
          value={licenseeId}
          options={licenseeOptions}
          onChange={(event) => {
            let value = normalize(event.target.value)
            setLicenseeId(value)

            //reset
            setBrandId(undefined)
          }}
          placeholder={licenseePlaceholder}
        />
      )}
      {isBrandActive && (
        <Select
          name={brandIdName}
          isLoading={isBrandDataLoading}
          value={brandId}
          options={brandOptions}
          onChange={(event) => {
            let value = normalize(event.target.value)
            setBrandId(value)
          }}
          placeholder={brandPlaceHolder}
        />
      )}
    </Group>
  )
}
