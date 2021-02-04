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
  Columns
} from "bumbag"
import { useIsDisplayMenuByRoute, useUserResellerId } from "../../user/context"
import Disclosure from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Update as UpdateMenu } from "./menu"
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
import { GameProviderCheckboxGroup } from "../../game/providers/GameProviderCheckboxGroup"
import { useReseller } from "../resellers/client"
import {
  mapChecked,
  mapCheckedById,
  mapCheckedByCultureCode,
  mapCheckedByCode
} from "../../checkbox/mapChecked"
import { LanguageCheckboxGroupTable } from "../../language/LanguageCheckboxGroup"
import { CurrencyCheckboxGroupTable } from "../../currency/CurrencyCheckboxGroup"
import { BetLimitCheckboxGroup } from "../../betlimit/BetLimitCheckboxGroup"
import { allCheckedKeys } from "../../checkbox/allCheckedKeys"
import { MerchantIdSelect } from "../MerchantSelect"
import { RoleTypes } from "../../system/roles/type"
import CurrencyFieldsBox, { toCheckedCurrencyCodes } from "../CurrencyFieldsBox"

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
      let gameProviders = mapCheckedById([], reseller.gameProviders)
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
  let { result, isLoading, error, mutate } = useAddLicensee(formData)

  const onSubmit = (data) => {
    setSubmit(normalizeFormData(data))
  }

  const reset = () => {
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
        <ConfirmButtonFooter isLoading={isLoading} disabled={isLoading} />
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

  /* TODO: view auth?
  let isDisplay = useIsDisplayMenuByRoute([ViewMenu])
  if (!isDisplay) {
    return null
  }
  */
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

const useResellerLicensee = ({ id = "", resellerId = "" }) => {
  let {
    data: licensee,
    isLoading: isLoadingLicensee,
    error: licenseeError,
    mutate: mutateLicensee,
    deleteCache: deleteLicenseeCache
  } = useLicensee(id)
  let {
    data: reseller,
    isLoading: isLoadingReseller,
    error: resellerError
  } = useReseller(resellerId)

  let languages = mapCheckedByCultureCode(
    licensee?.languages ?? [],
    reseller?.languages ?? []
  )
  let currencies = mapCheckedByCode(
    licensee?.currencies ?? [],
    reseller?.currencies ?? []
  )
  let gameProviders = mapCheckedById(
    licensee?.gameProviders ?? [],
    reseller?.gameProviders ?? []
  )
  let betLimits = mapCheckedById(licensee?.betLimits, reseller?.betLimits ?? [])

  return {
    reseller,
    licensee,
    languages,
    currencies,
    gameProviders,
    betLimits,
    isLoading: isLoadingLicensee || isLoadingReseller,
    error: licenseeError || resellerError,
    mutateLicensee,
    deleteLicenseeCache
  }
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
  cultureCodes,
  currencies,
  gameProviderIds,
  betLimitIds,
  ...data
}) => {
  cultureCodes = allCheckedKeys(cultureCodes)

  let currencyCodes = toCheckedCurrencyCodes(currencies)

  gameProviderIds = allCheckedKeys(gameProviderIds)
  betLimitIds = allCheckedKeys(betLimitIds)
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
  let { result, isLoading, error, mutate } = useUpdateLicensee(id, formData)

  const onSubmit = (data) => {
    setSubmit(normalizeFormData(data))
  }

  const reset = () => {
    mutateLicensee(undefined, false)
    deleteLicenseeCache()

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
      <AlertDialog {...licenseeError} {...dialog} onClickClose={dialog.hide} />
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
        <ConfirmButtonFooter isLoading={isLoading} disabled={isLoading} />
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
  let code = watch("code")
  let languages = watch("languages")

  let gameProviders = watch("gameProviders")
  let betLimits = watch("betLimits")

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
              {isAdd && <MerchantIdSelect end={RoleTypes.Reseller} />}
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
          {!isAdd && <Text.Block>{code}</Text.Block>}
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
          {editable && <Input type="email" name="operationalEmail" required />}
          {readonly && <Text.Block>{record.operationalEmail}</Text.Block>}
        </Box>

        {languages && (
          <>
            <NormalLabel>
              <Trans zh="语言" en="Language" />
            </NormalLabel>
            <LanguageCheckboxGroupTable disabled={readonly} />
          </>
        )}

        <CurrencyFieldsBox readonly={readonly} />

        {gameProviders && (
          <>
            <NormalLabel>
              <Trans zh="游戏厂商" en="Game Provider" />
            </NormalLabel>
            <Box>
              <GameProviderCheckboxGroup
                name="gameProviderIds"
                fieldArrayName="gameProviders"
                marginTop="major-1"
                disabled={readonly}
              />
            </Box>
          </>
        )}

        {betLimits && (
          <>
            <NormalLabel>
              <Trans zh="游戏商限红" en="Game Provider Bet Limit" />
            </NormalLabel>
            <FieldStack marginTop="major-1">
              <BetLimitCheckboxGroup disabled={readonly} />
            </FieldStack>
          </>
        )}
      </FieldValues>
    </Stack>
  )
}
