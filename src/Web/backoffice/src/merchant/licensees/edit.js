import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
    useModalState,
    Stack,
    Heading,
    Text,
    Box,
    Set,
    FieldStack,
    Columns,
    Code
} from "bumbag"
import { useIsDisplayMenuByRoute, useUserResellerId } from "../../user/context"
import Disclosure from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Update as UpdateMenu, View as ViewMenu } from "./menu"
import { Trans, useTrans } from "../../i18n"
import { useFormContext, useFieldArray, useForm } from "react-hook-form"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import { useAddLicensee, useLicensee, useUpdateLicensee } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import {
    CloseButtonFooter,
    ConfirmButtonFooter
} from "../../form/ConfirmButtonFooter"
import Input from "../../input/Input"
import Textarea from "../../textarea"
import { toCheckedGameProviderIds } from "../../game/providers/GameProviderCheckboxGroup"
import { useReseller } from "../resellers/client"
import {
    mapCheckedById,
    mapCheckedByCultureCode,
    mapCheckedByCode,
    mapCheckedByProp
} from "../../checkbox/mapChecked"
import { MerchantIdSelect } from "../MerchantSelect"
import { RoleTypes } from "../../system/roles/type"
import { toCheckedCurrencyCodes } from "../CurrencyFieldsBox"
import LanguageFieldsBox, { toCheckedCultureCodes } from "../LanguageFieldsBox"
import GameProviderFieldsBox from "../GameProviderFieldsBox"
import useResellerLicensee from "./useResellerLicensee"
import CurrencyBetLimitFieldsBox, {
    flatCheckedCurrencyBetLimitIds
} from "../CurrencyBetLimitFieldsBox"

export const AddButton = ({ onSubmitted, ...props }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog}>
                <Trans zh="新增" en="Add" />
            </Disclosure>
            {dialog.visible && (
                <AddDialog onSubmitted={onSubmitted} dialog={dialog} />
            )}
        </>
    )
}

const AddDialog = ({ onSubmitted, dialog }) => {
    let userResellerId = useUserResellerId()
    let formMethods = useForm({
        defaultValues: {
            resellerId: userResellerId
        }
    })
    let { watch } = formMethods
    let resellerId = watch("resellerId")
    let {
        data: reseller,
        isLoading: isLoadingReseller,
        error: resellerError
    } = useReseller(resellerId)

    useEffect(() => {
        if (reseller) {
            let languages = mapCheckedByCultureCode([], reseller.languages)
            let currencies = mapCheckedByCode([], reseller.currencies)
            let gameProviders = mapCheckedByProp(
                "gameProviderId",
                [],
                reseller.gameProviders
            )

            let betLimits = mapCheckedById([], reseller.betLimits)
            formMethods.reset({
                resellerId,
                languages,
                currencies,
                gameProviders,
                betLimits
            })
        }
    }, [reseller])

    let [formData, setSubmit] = useState()
    let { result, isLoading, error, mutate, deleteCache } =
        useAddLicensee(formData)

    const onSubmit = (data) => {
        let formData = normalizeFormData(data)
        if (userResellerId) {
            formData.resellerId = userResellerId
        }
        setSubmit(formData)
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    let title = useTrans({
        zh: "新增公司",
        en: "Add Licensee"
    })

    return (
        <FormDialog
            formMethods={formMethods}
            onSubmit={onSubmit}
            result={result}
            isLoading={isLoading}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={() => {
                reset()
                onSubmitted?.()
            }}
            showActionButtons={false}
            footer={
                <ConfirmButtonFooter
                    isLoading={isLoading}
                    disabled={isLoading}
                />
            }
            dialog={dialog}
            title={title}
            placement="top"
            minWidth="60vw"
            maxWidth="unset"
            css={css`
                & .bb-DialogContent {
                    position: relative;
                }
            `}
        >
            <EditBody isAdd />
        </FormDialog>
    )
}

export const ViewButton = ({ record, ...props }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([ViewMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog} {...props}>
                <Trans zh="查看" en="View" />
            </Disclosure>
            {dialog.visible && (
                <ViewDialog
                    id={record?.id}
                    resellerId={record?.resellerId}
                    record={record}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const ViewDialog = ({ id, resellerId, dialog, ...props }) => {
    let {
        licensee,
        reseller,
        languages,
        currencies,
        gameProviders,
        betLimits,
        isLoading,
        error,
        mutateLicensee,
        deleteLicenseeCache
    } = useResellerLicensee({ id, resellerId })

    let title = useTrans({
        zh: "查看公司",
        en: "View Licensee"
    })

    if (isLoading) {
        return <LoadingBackdrop {...dialog} onClickClose={dialog.hide} />
    }

    if (error) {
        return <AlertDialog {...error} {...dialog} onClickClose={dialog.hide} />
    }

    const reset = () => {
        mutateLicensee(undefined, false)
        deleteLicenseeCache()
    }

    let record = {
        ...licensee,
        reseller,
        languages,
        currencies,
        gameProviders,
        betLimits
    }
    return (
        <FormDialog
            defaultValues={record}
            title={title}
            dialog={dialog}
            onClickClose={reset}
            showActionButtons={false}
            footer={<CloseButtonFooter dialog={dialog} onClickClose={reset} />}
            placement="top"
            minWidth="60vw"
            maxWidth="unset"
            css={css`
                & .bb-DialogContent {
                    position: relative;
                }
            `}
        >
            <EditBody readonly record={record} />
        </FormDialog>
    )
}

export const EditButton = ({ record, onSubmitted, ...props }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog}>
                <Trans zh="修改" en="Edit" />
            </Disclosure>
            {dialog.visible && (
                <EditDialog
                    id={record?.id}
                    resellerId={record?.resellerId}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const normalizeFormData = ({
    languages,
    currencies,
    gameProviders,
    betLimits,
    ...data
}) => {
    let cultureCodes = toCheckedCultureCodes(languages)

    let currencyCodes = toCheckedCurrencyCodes(currencies)

    let gameProviderIds = toCheckedGameProviderIds(
        "gameProviderId",
        gameProviders
    )

    let betLimitIds = flatCheckedCurrencyBetLimitIds(currencies)
    return {
        ...data,
        cultureCodes,
        currencyCodes,
        gameProviderIds,
        betLimitIds
    }
}

const EditDialog = ({ id, resellerId, onSubmitted, dialog, ...props }) => {
    let {
        licensee,
        reseller,
        languages,
        currencies,
        gameProviders,
        betLimits,
        isLoading: isLoadingLicensee,
        error: licenseeError,
        mutateLicensee,
        deleteLicenseeCache
    } = useResellerLicensee({ id, resellerId })
    let [formData, setSubmit] = useState()
    let { result, isLoading, error, mutate, deleteCache } = useUpdateLicensee(
        id,
        formData
    )

    const onSubmit = (data) => {
        let formData = normalizeFormData(data)
        setSubmit(formData)
    }

    const reset = () => {
        mutateLicensee(undefined, false)
        deleteLicenseeCache()

        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    let title = useTrans({
        zh: "编辑公司",
        en: "Edit Licensee"
    })

    if (isLoadingLicensee) {
        return <LoadingBackdrop {...dialog} onClickClose={dialog.hide} />
    }

    if (licenseeError) {
        return (
            <AlertDialog
                {...licenseeError}
                {...dialog}
                onClickClose={dialog.hide}
            />
        )
    }

    let record = {
        ...licensee,
        reseller,
        languages,
        currencies,
        gameProviders,
        betLimits
    }
    return (
        <FormDialog
            defaultValues={record}
            onSubmit={onSubmit}
            result={result}
            isLoading={isLoading}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={() => {
                reset()
                onSubmitted?.()
            }}
            onClickClose={reset}
            title={title}
            showActionButtons={false}
            footer={
                <ConfirmButtonFooter
                    isLoading={isLoading}
                    disabled={isLoading}
                />
            }
            dialog={dialog}
            placement="top"
            minWidth="60vw"
            maxWidth="unset"
            css={css`
                & .bb-DialogContent {
                    position: relative;
                }
            `}
        >
            <EditBody record={record} />
        </FormDialog>
    )
}

const EditBody = ({
    record,
    readonly = false,
    isAdd = false,
    addonBefore,
    ...props
}) => {
    let { watch, getValues, register } = useFormContext()

    let userResellerId = useUserResellerId()
    let reseller = watch("reseller")

    let editable = !readonly
    return (
        <Stack orientation="vertical">
            <Heading use="h5" marginBottom="major-1">
                <Trans zh="基本资料" en="Basic Information" />
            </Heading>
            <FieldValues>
                {!userResellerId && (
                    <>
                        <NormalLabel>
                            <Trans zh="所属总公司" en="Licensee" />
                        </NormalLabel>
                        <Box alignY="center">
                            {isAdd && (
                                <MerchantIdSelect end={RoleTypes.Reseller} />
                            )}
                            {!isAdd && <Text.Block>{reseller.name}</Text.Block>}
                        </Box>
                    </>
                )}

                <NormalLabel>
                    <Trans zh="公司代码" en="Code" />
                </NormalLabel>
                <Box alignY="center">
                    <Input
                        name="code"
                        type={isAdd ? "input" : "hidden"}
                        required={isAdd}
                    />
                    {!isAdd && <Text.Block>{getValues("code")}</Text.Block>}
                </Box>

                <NormalLabel>
                    <Trans zh="公司名称" en="Name" />
                </NormalLabel>
                <Box alignY="center">
                    {editable && <Input name="name" required />}
                    {readonly && <Text.Block>{record?.name}</Text.Block>}
                </Box>

                <NormalLabel>
                    <Trans zh="公司简介" en="Description" />
                </NormalLabel>
                <Box alignY="center">
                    {editable && <Textarea name="description" />}
                    {readonly && <Text.Block>{record.description}</Text.Block>}
                </Box>

                <NormalLabel>
                    <Trans zh="公司邮箱" en="Email" />
                </NormalLabel>
                <Box alignY="center">
                    {editable && (
                        <Input type="email" name="operationalEmail" required />
                    )}
                    {readonly && (
                        <Text.Block>{record.operationalEmail}</Text.Block>
                    )}
                </Box>

                <LanguageFieldsBox readonly={readonly} />

                <CurrencyBetLimitFieldsBox readonly={readonly} />

                <GameProviderFieldsBox readonly={readonly} />
            </FieldValues>
        </Stack>
    )
}
