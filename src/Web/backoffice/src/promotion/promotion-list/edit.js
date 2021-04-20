import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import {
    Box,
    Text,
    Button,
    Set,
    Icon,
    useModalState,
    Dialog,
    Card,
    Flex,
    Heading,
    FieldStack
} from "bumbag"
import { Update as UpdateMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import {
    AddDisclosure,
    EditDisclosure,
    ViewDisclosure
} from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Trans, useTrans } from "../../i18n"
import { useForm, useFormContext } from "react-hook-form"
import { InputField, Input } from "../../input/Input"
import clone from "clone-deep"
import { useAddPromotion, useUpdatePromotion } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import { DateInputRange } from "../../date/DateTimeRangeInput"
import { VipLevelCheckboxGroup } from "../../vip-levels/CheckboxGroup"
import AlertDialog from "../../dialog/Alert"
import Checkbox from "../../checkbox"
import {
    GameTypeGroupSelect,
    GameTypeSelect,
    useGameTypeGroupValueOf
} from "../../game/GameType"
import Separator from "../../layout/Separator"
import {
    PromotionTypeGroupSelect,
    PromotionTypeSelect,
    usePromotionGroupValueOf
} from "./PromotionType"
import { toCheckedGameProviderIds } from "../../game/providers/GameProviderCheckboxGroup"
import { BrandGameProviderCheckboxGroupTable } from "../../game/brandproviders/BrandGameProviderCheckboxGroup"
import parseISO from "date-fns/parseISO"
import { today } from "../../date/range"
import { useBrandGameProviderList } from "../../game/brandproviders/client"
import { CloseButtonFooter } from "../../form/ConfirmButtonFooter"

export const AddPromotionDisclosure = ({ brandId, onSubmitted }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <AddDisclosure {...dialog} />
            {dialog.visible && (
                <AddDialog
                    brandId={brandId}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const AddDialog = ({ brandId, onSubmitted, dialog, ...props }) => {
    let [formData, setSubmit] = useState()

    let { result, isLoading, error, mutate, deleteCache } = useAddPromotion(
        formData
    )

    let title = useTrans({
        zh: "新增活动",
        en: "Create Promotion"
    })

    const onSubmit = (data) => {
        let formData = normalizeFormData(data)
        setSubmit({
            ...formData,
            brandId
        })
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <FormDialog
            defaultValues={{}}
            onSubmit={onSubmit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={onSubmitted}
            title={title}
            dialog={dialog}
            maxWidth="80vw"
            css={css`
                & .bb-DialogContent {
                    position: relative;
                    padding: 1.5rem 3rem;
                }
            `}
        >
            <FormBody brandId={brandId} isAdd />
        </FormDialog>
    )
}

const normalizeFormData = ({ gameProviders = [], ...data }) => {
    let gameProviderIds = toCheckedGameProviderIds(
        "gameProviderId",
        gameProviders
    )

    return {
        gameProviderIds,
        ...data
    }
}

export const EditPromotionDisclosure = ({
    brandId,
    record,
    onSubmitted,
    ...props
}) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <EditDisclosure {...dialog} />
            {dialog.visible && (
                <EditDialog
                    brandId={brandId}
                    record={record}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const EditDialog = ({ brandId, record, onSubmitted, dialog, ...props }) => {
    let [formData, setSubmit] = useState()

    let {
        isLoading: isLoadingFormData,
        data,
        error: loadingFormDataError
    } = usePromotionFormData(brandId, record)
    let { result, isLoading, error, mutate, deleteCache } = useUpdatePromotion(
        record?.id,
        formData
    )

    let title = useTrans({
        zh: "编辑活动",
        en: "Edit Promotion"
    })

    if (isLoadingFormData) {
        return <LoadingBackdrop {...dialog} />
    }

    if (loadingFormDataError) {
        return (
            <AlertDialog {...loadingFormDataError} onClickClose={dialog.hide} />
        )
    }

    const onSubmit = (data) => {
        setSubmit({
            ...normalizeFormData(data),
            brandId
        })
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <FormDialog
            defaultValues={clone(data)}
            onSubmit={onSubmit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={onSubmitted}
            title={title}
            dialog={dialog}
            maxWidth="80vw"
            css={css`
                & .bb-DialogContent {
                    position: relative;
                    padding: 1.5rem 3rem;
                }
            `}
        >
            <FormBody brandId={brandId} />
        </FormDialog>
    )
}

const usePromotionFormData = (
    brandId,
    { from, to, gameProviderIds = [], vipLevels = [], ...record } = {}
) => {
    let { data = {}, ...rest } = useBrandGameProviderList(
        brandId
            ? {
                  brandId,
                  showAll: true
              }
            : undefined
    )

    let gameProviders =
        data?.items?.map((gameProvider) => {
            let checked = gameProviderIds.includes(gameProvider.gameProviderId)
            return {
                ...gameProvider,
                checked
            }
        }) ?? []

    let promotionTypeGroup = usePromotionGroupValueOf(record?.promotionType)
    let gameTypeGroup = useGameTypeGroupValueOf(record?.gameType)

    vipLevels = vipLevels.map((vipLevel) => `${vipLevel}`)

    data = {
        ...record,
        vipLevels,
        gameProviders,
        promotionTypeGroup,
        gameTypeGroup,
        from: parseISO(from),
        to: parseISO(to)
    }

    return {
        ...rest,
        data
    }
}

export const ViewPromotionDisclosure = ({
    brandId,
    record,
    onSubmitted,
    ...props
}) => {
    let dialog = useModalState()

    // let isDisplay = useIsDisplayMenuByRoute([ViewMenu])
    // if (!isDisplay) {
    //     return null
    // }

    return (
        <>
            <ViewDisclosure {...dialog} />
            {dialog.visible && (
                <ViewDialog brandId={brandId} record={record} dialog={dialog} />
            )}
        </>
    )
}

const ViewDialog = ({ brandId, record, dialog, ...props }) => {
    let { isLoading, data, error } = usePromotionFormData(brandId, record)

    let title = useTrans({
        zh: "查看活动",
        en: "View Promotion"
    })

    if (isLoading) {
        return <LoadingBackdrop {...dialog} />
    }

    if (error) {
        return <AlertDialog onClickClose={dialog.hide} />
    }

    return (
        <FormDialog
            defaultValues={clone(data)}
            title={title}
            showActionButtons={false}
            footer={<CloseButtonFooter dialog={dialog} alignX="right" />}
            dialog={dialog}
            maxWidth="80vw"
            css={css`
                & .bb-DialogContent {
                    position: relative;
                    padding: 1.5rem 3rem;
                }
            `}
        >
            <FormBody brandId={brandId} record={record} readonly />
        </FormDialog>
    )
}

const FormBody = ({ brandId, record, readonly = false, isAdd = false }) => {
    let { start: startOfToday } = today()
    let { watch } = useFormContext()

    let totalAmountLimit = watch("totalAmountLimit")
    let unlimitedTotalAmount = watch("unlimited")
    let limitedTotalAmount = !unlimitedTotalAmount

    let unlimiteTotalAmountLabel = useTrans({
        zh: "无限",
        en: "Unlimited"
    })

    let promotionTypeGroup = watch("promotionTypeGroup")
    let gameTypeGroup = watch("gameTypeGroup")

    let promotionType = watch("promotionType")

    let disabled = readonly
    let editable = !readonly

    return (
        <FieldStack marginLeft="1rem">
            <FieldValues>
                <NormalLabel>
                    <Trans zh="活动名称" en="Promotion Name" />
                </NormalLabel>
                <Box alignY="center">
                    {editable && <Input name="name" />}
                    {readonly && (
                        <Text.Block alignY="center">{record?.name}</Text.Block>
                    )}
                </Box>

                <NormalLabel>
                    <Trans zh="活动时间" en="Promotion Time" />
                </NormalLabel>
                <Box alignY="center">
                    <DateInputRange
                        startProps={{
                            name: "from"
                        }}
                        endProps={{
                            name: "to",
                            minDate: startOfToday
                        }}
                        minDate={isAdd ? startOfToday : undefined}
                        disabled={disabled}
                    />
                </Box>

                <NormalLabel>
                    <Trans zh="活动用户群" en="Promotion VIP Levels" />
                </NormalLabel>
                <Box alignY="center">
                    <VipLevelCheckboxGroup
                        name="vipLevels"
                        valueFieldName="level"
                        brandId={brandId}
                        required
                        disabled={disabled}
                    />
                </Box>

                <NormalLabel>
                    <Trans zh="活动类型" en="Promotion Type Group" />
                </NormalLabel>
                <Box alignY="center">
                    <PromotionTypeGroupSelect
                        name="promotionTypeGroup"
                        disabled={disabled}
                    />
                </Box>

                <NormalLabel>
                    <Trans zh="活动子类型" en="Promotion Type" />
                </NormalLabel>
                <Box alignY="center">
                    <PromotionTypeSelect
                        name="promotionType"
                        group={promotionTypeGroup}
                        required
                        disabled={disabled}
                    />
                </Box>

                <NormalLabel>
                    <Trans zh="活动费用" en="Total Amount Limit" />
                </NormalLabel>
                <Set orientation="horizontal" alignY="center">
                    {unlimitedTotalAmount && (
                        <Box>
                            <Input name="totalAmountLimit" type="hidden" />
                            <Input value={totalAmountLimit} disabled />
                        </Box>
                    )}
                    {limitedTotalAmount && (
                        <>
                            <Input
                                type="number"
                                name="totalAmountLimit"
                                required
                                disabled={disabled}
                            />
                        </>
                    )}
                    <Checkbox
                        name="unlimited"
                        label={unlimiteTotalAmountLabel}
                        disabled={disabled}
                    />
                </Set>

                <NormalLabel>
                    <Trans zh="活动游戏类型" en="Game Type Group" />
                </NormalLabel>
                <GameTypeGroupSelect name="gameTypeGroup" disabled={disabled} />

                <NormalLabel>
                    <Trans zh="活动游戏子类型" en="Game Type" />
                </NormalLabel>
                <GameTypeSelect
                    name="gameType"
                    group={gameTypeGroup}
                    required
                    disabled={disabled}
                />

                <NormalLabel>
                    <Trans zh="活动厂商" en="Game Providers" />
                </NormalLabel>
                <Box>
                    <BrandGameProviderCheckboxGroupTable
                        brandId={brandId}
                        disabled={disabled}
                    />
                </Box>
            </FieldValues>

            <Separator />

            <Heading use="h5">
                <Trans zh="活动规则" en="Rules" />
            </Heading>

            <FieldValues>
                <NormalLabel>
                    <Trans zh="彩金百分比" en="Percentage" />
                </NormalLabel>
                <InputField
                    type="number"
                    step="0.01"
                    name="percentage"
                    min="0"
                    max="100"
                    required
                    disabled={disabled}
                    addonAfter={
                        <Button isStatic tabIndex="-1">
                            %
                        </Button>
                    }
                />

                <NormalLabel>
                    <Trans zh="彩金上限" en="Max Amount" />
                </NormalLabel>
                <InputField
                    type="number"
                    name="promotionAmount"
                    min="0"
                    required
                    disabled={disabled}
                />

                <NormalLabel>
                    <Trans zh="流水倍数" en="Turnover Multiple" />
                </NormalLabel>
                <InputField
                    type="number"
                    name="turnoverMultiple"
                    min="0"
                    required
                    disabled={disabled}
                    addonAfter={
                        <Button isStatic tabIndex="-1">
                            <Trans zh="倍" en="X" />
                        </Button>
                    }
                />
            </FieldValues>
        </FieldStack>
    )
}
