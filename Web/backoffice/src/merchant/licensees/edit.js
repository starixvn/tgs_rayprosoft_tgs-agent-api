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
      <EditBody
        isAdd
        addonBefore={
          <>
            <Box>{/* <Trans zh="总公司" en="Reseller" /> */}</Box>
            <MerchantIdSelect
              end={RoleTypes.Reseller}
              resellerId={resellerId}
            />
          </>
        }
      />
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
        <LoadingDataDialog
          id={record?.id}
          resellerId={record?.resellerId}
          dialog={dialog}
        >
          {({ licensee, reseller }) => {
            return (
              <ViewDialog
                record={record}
                licensee={licensee}
                reseller={reseller}
                dialog={dialog}
              />
            )
          }}
        </LoadingDataDialog>
      )}
    </>
  )
}

const ViewDialog = ({ licensee, reseller, resetData, dialog, ...props }) => {
  let title = useTrans({
    zh: "查看公司",
    en: "View Licensee"
  })

  let languages = mapCheckedByCultureCode(
    licensee.languages,
    reseller.languages
  )
  let currencies = mapCheckedByCode(licensee.currencies, reseller.currencies)
  let gameProviders = mapCheckedById(
    licensee.gameProviders,
    reseller.gameProviders
  )
  let betLimits = mapCheckedById(licensee.betLimits, reseller.betLimits)

  let record = {
    ...licensee,
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
      onClickClose={resetData}
      showActionButtons={false}
      footer={<CloseButtonFooter dialog={dialog} onClickClose={resetData} />}
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
        <LoadingDataDialog
          id={record?.id}
          resellerId={record?.resellerId}
          dialog={dialog}
        >
          {({ licensee, reseller, resetData }) => {
            return (
              <EditDialog
                record={record}
                licensee={licensee}
                reseller={reseller}
                resetData={resetData}
                onSubmitted={() => {
                  resetData()
                  onSubmitted?.()
                }}
                dialog={dialog}
              />
            )
          }}
        </LoadingDataDialog>
      )}
    </>
  )
}

const LoadingDataDialog = ({ id = "", resellerId = "", dialog, children }) => {
  let {
    data: licensee,
    isLoading: isLoadingLicensee,
    error: licenseeError,
    mutate,
    deleteCache
  } = useLicensee(id)
  let {
    data: reseller,
    isLoading: isLoadingReseller,
    error: resellerError
  } = useReseller(resellerId)

  if (isLoadingLicensee || isLoadingReseller) {
    return <LoadingBackdrop {...dialog} />
  }

  if (licenseeError) {
    return <AlertDialog {...licenseeError} onClickClose={dialog.hide} />
  }

  if (resellerError) {
    return <AlertDialog {...resellerError} onClickClose={dialog.hide} />
  }

  const resetData = () => {
    deleteCache()
    mutate(undefined, false)
  }

  return children({
    licensee,
    reseller,
    resetData
  })
}

const normalizeFormData = ({
  cultureCodes,
  currencyCodes,
  gameProviderIds,
  betLimitIds,
  ...data
}) => {
  cultureCodes = allCheckedKeys(cultureCodes)
  currencyCodes = allCheckedKeys(currencyCodes)
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

const EditDialog = ({
  record,
  licensee,
  reseller,
  resetData,
  onSubmitted,
  dialog,
  ...props
}) => {
  let [formData, setSubmit] = useState()
  let { result, isLoading, error, mutate } = useUpdateLicensee(
    record?.id,
    formData
  )

  const onSubmit = (data) => {
    setSubmit(normalizeFormData(data))
  }

  const reset = () => {
    mutate(undefined, false)
    setSubmit(undefined)
    resetData?.()
  }

  let title = useTrans({
    zh: "编辑公司",
    en: "Edit Licensee"
  })

  let languages = mapCheckedByCultureCode(
    licensee.languages,
    reseller.languages
  )
  let currencies = mapCheckedByCode(licensee.currencies, reseller.currencies)
  let gameProviders = mapCheckedById(
    licensee.gameProviders,
    reseller.gameProviders
  )
  let betLimits = mapCheckedById(licensee.betLimits, reseller.betLimits)
  return (
    <FormDialog
      defaultValues={{
        ...licensee,
        languages,
        currencies,
        gameProviders,
        betLimits
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
      <EditBody />
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

  let code = watch("code")
  let languages = watch("languages")
  let currencies = watch("currencies")
  let gameProviders = watch("gameProviders")
  let betLimits = watch("betLimits")

  let editable = !readonly
  return (
    <Stack orientation="vertical">
      <Heading use="h5" marginBottom="major-1">
        <Trans zh="基本资料" en="Basic Information" />
      </Heading>
      <FieldValues>
        {addonBefore}

        <NormalLabel>
          <Trans zh="公司代码" en="Code" />
        </NormalLabel>
        <Box alignY="center">
          <Input name="code" type={isAdd ? "input" : "hidden"} />
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

        {currencies && (
          <>
            <NormalLabel>
              <Trans zh="币种" en="Currency" />
            </NormalLabel>
            <CurrencyCheckboxGroupTable disabled={readonly} />
          </>
        )}

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
