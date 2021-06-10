import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext, useFieldArray } from "react-hook-form"
import {
    Stack,
    Heading,
    Box,
    Button,
    Text,
    Group,
    Set,
    Flex,
    useModalState
} from "bumbag"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { View, Update as UpdateMenu } from "./menu"
import {
    AddDisclosure,
    EditDisclosure,
    ViewDisclosure
} from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Trans, useTrans } from "../../i18n"
import {
    useAddGameProvider,
    useGameProvider,
    useUpdateGameProvider
} from "../../game/providers/client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel } from "../../form/Label"
import { Input } from "../../input/Input"
import { Textarea } from "../../textarea"
import GameProviderStatus, {
    GameProviderStatusSelect
} from "./GameProviderStatus"
import { useLanguageList } from "../../language/client"
import { useCurrencyList } from "../../currency/client"
import { GameProviderLanguageCheckboxGroupTable } from "./GameProviderLanguageCheckboxGroup"
import {
    mapCheckedByCode,
    mapCheckedByCultureCode,
    mapMergeChecked
} from "../../checkbox/mapChecked"
import ConfirmButtonFooter, {
    CloseButtonFooter
} from "../../form/ConfirmButtonFooter"
import Checkbox from "../../checkbox"
import { GameProviderCurrencyBetLimitFieldsBox } from "./GameProviderCurrencyBetLimitFieldsBox"
import clone from "clone-deep"

export const ViewGameProviderDisclosure = ({ gameProviderId }) => {
    let dialog = useModalState()
    let isDisplay = useIsDisplayMenuByRoute([View])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <ViewDisclosure {...dialog} />
            {dialog.visible && (
                <ViewDialog gameProviderId={gameProviderId} dialog={dialog} />
            )}
        </>
    )
}

const ViewDialog = ({ gameProviderId, dialog }) => {
    let {
        gameProvider,
        languages,
        currencies,
        isLoading,
        error,
        mutateGameProvider,
        deleteGameProviderCache
    } = useFormDataInit(gameProviderId)

    let title = useTrans({
        zh: "游戏商",
        en: "Game Provider"
    })

    if (isLoading) {
        return <LoadingBackdrop {...dialog} />
    }

    if (error) {
        return <AlertDialog {...error} onClickClose={dialog.hide} />
    }

    return (
        <FormDialog
            defaultValues={{ ...gameProvider, languages, currencies }}
            footer={<CloseButtonFooter dialog={dialog} alignX="right" />}
            showActionButtons={false}
            title={title}
            dialog={dialog}
            placement="top"
            minWidth="80vw"
            maxWidth="90vw"
            css={css`
                & .bb-DialogContent {
                    position: relative;
                }
            `}
        >
            <FormBody readOnly />
        </FormDialog>
    )
}

export const EditGameProviderButton = ({
    gameProviderId,
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
                    gameProviderId={gameProviderId}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

export const AddGameProviderButton = ({ onSubmitted, ...props }) => {
    let dialog = useModalState()
    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <AddDisclosure {...dialog} />
            {dialog.visible && (
                <AddDialog onSubmitted={onSubmitted} dialog={dialog} />
            )}
        </>
    )
}

const AddDialog = ({ onSubmitted, dialog, ...props }) => {
    let {
        languages,
        currencies,
        isLoading: isLoadingFormData,
        error: formDataError
    } = useFormDataInit()
    let [formData, setSubmit] = useState()
    let { result, isLoading, error, mutate, deleteCache } =
        useAddGameProvider(formData)

    let title = useTrans({
        zh: "新增游戏商",
        en: "Add Game Provider"
    })

    if (isLoadingFormData) {
        return <LoadingBackdrop {...dialog} />
    }

    if (formDataError) {
        return <AlertDialog {...formDataError} onClickClose={dialog.hide} />
    }

    const onSubmit = (data) => {
        let formData = normalizeFormData(data)
        setSubmit(formData)
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <FormDialog
            defaultValues={{
                status: GameProviderStatus.Active,
                languages,
                currencies
            }}
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
            dialog={dialog}
            placement="top"
            minWidth="80vw"
            maxWidth="90vw"
            css={css`
                & .bb-DialogContent {
                    position: relative;
                }
            `}
        >
            <FormBody isAdd={true} />
        </FormDialog>
    )
}

const EditDialog = ({ gameProviderId, onSubmitted, dialog, ...props }) => {
    let {
        gameProvider,
        languages,
        currencies,
        isLoading: isLoadingFormData,
        error: formDataError,
        mutateGameProvider,
        deleteGameProviderCache
    } = useFormDataInit(gameProviderId)
    let [formData, setSubmit] = useState()
    let { result, isLoading, error, mutate, deleteCache } =
        useUpdateGameProvider(gameProviderId, formData)

    let title = useTrans({
        zh: "编辑游戏商",
        en: "Edit Game Provider"
    })

    if (isLoadingFormData) {
        return <LoadingBackdrop {...dialog} />
    }

    if (formDataError) {
        return <AlertDialog {...formDataError} onClickClose={dialog.hide} />
    }

    const onSubmit = (data) => {
        let formData = normalizeFormData(data)
        setSubmit(formData)
    }

    const reset = () => {
        mutateGameProvider(undefined, false)
        deleteGameProviderCache()

        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <FormDialog
            defaultValues={{ ...gameProvider, languages, currencies }}
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
            dialog={dialog}
            placement="top"
            minWidth="80vw"
            maxWidth="90vw"
            css={css`
                & .bb-DialogContent {
                    position: relative;
                }
            `}
        >
            <FormBody />
        </FormDialog>
    )
}

const normalizeFormData = ({ currencies, languages, ...data }) => {
    currencies = Object.entries(currencies).reduce(
        (list, [key, { checked, ...props }]) => {
            if (checked) {
                return [...list, { code: key, ...props }]
            }

            return list
        },
        []
    )

    languages = Object.entries(languages).reduce(
        (list, [key, { checked, ...props }]) => {
            if (checked) {
                return [...list, { cultureCode: key, ...props }]
            }

            return list
        },
        []
    )

    return {
        currencies,
        languages,
        ...data
    }
}

const FormBody = ({ readOnly, isAdd = false }) => {
    //TODO: get value from id field -> isAdd
    let { watch, getValues } = useFormContext()

    let editable = !readOnly
    let disabled = readOnly
    return (
        <Stack orientation="vertical" marginLeft="1rem">
            <Heading use="h5" marginBottom="major-1">
                <Trans zh="基本资料" en="Basic Information" />
            </Heading>

            <FieldValues>
                <NormalLabel>
                    <Trans zh="游戏商代码" en="Code" />
                </NormalLabel>
                <Box alignY="center">
                    <Input name="code" readOnly={readOnly || !isAdd} />
                </Box>

                <NormalLabel>
                    <Trans zh="游戏商名称" en="Name" />
                </NormalLabel>
                <Box alignY="center">
                    <Input name="name" required readOnly={readOnly} />
                </Box>

                <NormalLabel>
                    <Trans zh="游戏商简介" en="Description" />
                </NormalLabel>
                <Input name="description" readOnly={readOnly} />

                <NormalLabel>
                    <Trans zh="状态" en="Status" />
                </NormalLabel>
                <GameProviderStatusSelect disabled={disabled} />

                <NormalLabel>
                    <Trans zh="Auto Close Round" en="Auto Close Round" />
                </NormalLabel>
                <Checkbox name="autoCloseRound" disabled={disabled} />

                <NormalLabel>
                    <Trans zh="Transfer Wallet" en="Transfer Wallet" />
                </NormalLabel>
                <Checkbox name="isTransferWallet " disabled={disabled} />

                <NormalLabel>
                    <Trans zh="Support Test Player" en="Support Test Player" />
                </NormalLabel>
                <Checkbox name="supportTestPlayer" disabled={disabled} />

                <NormalLabel>
                    <Trans zh="Allow Kick Player" en="Allow Kick Player" />
                </NormalLabel>
                <Checkbox name="allowKickPlayer" disabled={disabled} />

                <NormalLabel>
                    <Trans zh="Place Bet Timeout" en="Place Bet Timeout" />
                </NormalLabel>
                <Input name="placeBetTimeout" readOnly={readOnly} />

                <NormalLabel>
                    <Trans zh="Settle Bet Timeout" en="Settle Bet Timeout" />
                </NormalLabel>
                <Input name="settleBetTimeout" readOnly={readOnly} />

                <LanguageFieldsBox disabled={disabled} />

                <GameProviderCurrencyBetLimitFieldsBox disabled={readOnly} />
            </FieldValues>
        </Stack>
    )
}

const LanguageFieldsBox = ({ disabled }) => {
    let { control } = useFormContext()

    let fieldsName = "languages"
    let fieldArrayContext = useFieldArray({
        control,
        name: fieldsName,
        keyName: "_id"
    })

    return (
        <>
            <NormalLabel>
                <Trans zh="支持语言" en="Languages" />
            </NormalLabel>
            <GameProviderLanguageCheckboxGroupTable
                fieldArrayContext={fieldArrayContext}
                disabled={disabled}
            />
        </>
    )
}

const useFormDataInit = (gameProviderId) => {
    let {
        data: gameProvider,
        isLoading: isLoadingGameProvider,
        error: gameProviderError,
        mutate: mutateGameProvider,
        deleteCache: deleteGameProviderCache
    } = useGameProvider(gameProviderId)
    let {
        data: languageData,
        isLoading: isLoadingLanguage,
        error: languageError
    } = useLanguageList({ showAll: true, sortby: "cultureCode" })
    let {
        data: currencyData,
        isLoading: isLoadingCurrency,
        error: currencyError
    } = useCurrencyList({ showAll: true })

    let languages = mapMergeChecked(
        (gameProviderLang, lang) => {
            return gameProviderLang.cultureCode === lang.cultureCode
        },
        (gameProviderLang, lang) => {
            return {
                ...clone(lang),
                gameProviderCultureCode: lang.cultureCode,
                ...clone(gameProviderLang)
            }
        },
        gameProvider?.languages ?? [],
        languageData?.items ?? []
    )

    let currencies = mapMergeChecked(
        (gameProviderCurrency, currency) => {
            return gameProviderCurrency.code === currency.code
        },
        (gameProviderCurrency, currency) => ({
            ...clone(currency),
            ...clone(gameProviderCurrency)
        }),
        gameProvider?.currencies ?? [],
        currencyData?.items ?? []
    )
    return {
        gameProvider: clone(gameProvider),
        languages,
        currencies,
        isLoading:
            isLoadingGameProvider || isLoadingLanguage || isLoadingCurrency,
        error: gameProviderError || languageError || currencyError,
        mutateGameProvider,
        deleteGameProviderCache
    }
}
