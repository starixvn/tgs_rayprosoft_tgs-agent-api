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
import { ResellerIdSelect } from "./resellers/ResellerIdSelect"
import { LicenseeIdSelect } from "./licensees/LicenseeIdSelect"
import { BrandIdSelect } from "./brands/BrandIdSelect"

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

const ResellerIdName = "resellerId"
const DefaultResellerProps = {
    name: ResellerIdName
}

const LicenseeIdName = "licenseeId"
const DefaultLicenseeProps = {
    name: LicenseeIdName
}

const BrandIdName = "brandId"
const DefaultBrandProps = {
    name: BrandIdName
}

export const MerchantIdSelect = ({
    start = Reseller,
    end = Brand,

    resellerProps: {
        name: resellerIdName = ResellerIdName,
        onChange: onResellerChange,
        ...resellerProps
    } = DefaultResellerProps,

    licenseeProps: {
        name: licenseeIdName = LicenseeIdName,
        onChange: onLicenseeChange,
        ...licenseeProps
    } = DefaultLicenseeProps,

    brandProps: {
        name: brandIdName = BrandIdName,
        onChange: onBrandChange,
        ...brandProps
    } = DefaultBrandProps,

    required = true,
    parentRequired = false,
    disabled,
    ...props
}) => {
    let formMethods = useFormContext()

    let [resellerId, setResellerId] = useState(
        resellerProps?.value ?? resellerProps?.defaultValue
    )
    let isResellerActive = isActive(Reseller, start, end)
    if (
        isResellerActive &&
        parentRequired &&
        formMethods &&
        !resellerProps.validate
    ) {
        resellerProps.validate = (value) => {
            if (
                formMethods.getValues(brandIdName) ||
                formMethods.getValues(licenseeIdName)
            ) {
                return !!value
            }
        }
    }

    let [licenseeId, setLicenseeId] = useState(
        licenseeProps?.value ?? licenseeProps?.defaultValue
    )
    let isLicenseeActive = isActive(Licensee, start, end)
    if (
        isLicenseeActive &&
        parentRequired &&
        formMethods &&
        !licenseeProps.validate
    ) {
        licenseeProps.validate = (value) => {
            if (formMethods.getValues(brandIdName)) {
                return !!value
            }
        }
    }

    let isBrandActive = isActive(Brand, start, end)
    return (
        <Group {...props}>
            {isResellerActive && (
                <ResellerIdSelect
                    name={resellerIdName}
                    required={required}
                    disabled={disabled}
                    {...resellerProps}
                    onChange={(event) => {
                        formMethods?.setValue(licenseeIdName, undefined)
                        formMethods?.setValue(brandIdName, undefined)

                        let resellerId = event.target.value
                        setResellerId(resellerId)
                        onResellerChange?.(resellerId)
                    }}
                />
            )}
            {isLicenseeActive && (
                <LicenseeIdSelect
                    name={licenseeIdName}
                    required={required}
                    disabled={disabled}
                    resellerId={resellerId}
                    {...licenseeProps}
                    onChange={(event) => {
                        formMethods?.setValue(brandIdName, undefined)

                        let licenseeId = event.target.value
                        setLicenseeId(licenseeId)
                        onLicenseeChange?.(licenseeId)
                    }}
                />
            )}
            {isBrandActive && (
                <BrandIdSelect
                    name={brandIdName}
                    required={required}
                    disabled={disabled}
                    licenseeId={licenseeId}
                    {...brandProps}
                    onChange={(event) => {
                        let value = event.target.value
                        onBrandChange?.(value)
                    }}
                />
            )}
        </Group>
    )
}

export const MerchantIdHookFormSelect = ({
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
