import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import { Box, Button, Set, Icon } from "bumbag"
import { faCheck } from "@fortawesome/free-solid-svg-icons"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import { AddButton, EditButton } from "./edit"
import { CopyButton } from "./copy"
import { DeleteButton } from "./delete"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute,
  useUserRoleType,
  useUserRoleId
} from "../../user/context"
import { Code } from "./menu"
import { useRoleList } from "./client"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { useForm, useFormContext, FormProvider } from "react-hook-form"
import { InputField } from "../../input/Input"

export { Code }

export const Content = ({}) => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = () => {
  let { state, setState, setPageIndex, setSortOrder } = useTable({
    name: ""
  })
  let { pageIndex, pageSize, name } = state
  let { data, isLoading, mutate } = useRoleList(toApiParams(state))
  let userRoleId = useUserRoleId()
  let items = data?.items.filter(({ id }) => id != userRoleId)

  let columns = useColumns({ mutate })
  return (
    <PageContent
      title={<Trans zh="权限组管理" en="Roles" />}
      actionBar={
        <>
          <SearchBar
            search={(data) => {
              let newState = {
                pageIndex: 0,
                name: data?.name
              }
              if (equals(newState, { pageIndex, name })) {
                mutate()
              }
              setState(newState)
            }}
            isPending={isLoading}
          />
          <AddButton onSubmitted={mutate} />
        </>
      }
    >
      <Table
        controlledState={state}
        columns={columns}
        data={items}
        pageCount={pageCount(pageSize, data?.totalCount)}
        setPageIndex={setPageIndex}
        setSortOrder={setSortOrder}
        isLoading={isLoading}
      />
    </PageContent>
  )
}

const SearchBar = ({ search, isLoading, ...props }) => {
  let formMethods = useForm({
    defaultValues: { name: "" }
  })

  return (
    <FormProvider {...formMethods}>
      <form onSubmit={formMethods.handleSubmit(search)}>
        <Set>
          <InputField
            name="name"
            addonAfter={
              <Button onClick={() => formMethods.setValue("name", "")}>
                X
              </Button>
            }
            placeholder={useSearchInputPlaceHolder()}
            autocomplete="off"
          />
          <SearchButton isLoading={isLoading} />
        </Set>
      </form>
    </FormProvider>
  )
}

const useSearchInputPlaceHolder = () =>
  useTrans({
    zh: "权限组名称输入框",
    en: "Input role name"
  })

const useColumns = ({ mutate }) => {
  let columns = [
    sequenceColumn(),
    {
      id: "name",
      accessor: "name",
      Header: "权限组名称",
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      width: 400
    },
    {
      id: "isRoot",
      accessor: "isRoot",
      Header: "根管理权限",
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value: isRoot }) => {
        return (
          <Box alignX="center">
            {isRoot ? <Icon icon={faCheck} type="font-awesome" /> : ""}
          </Box>
        )
      }
    }
  ]

  let hasAnyMenu = useHasSubMenuByRoute()
  if (hasAnyMenu) {
    columns.push({
      disableSortBy: true,
      accessor: (row) => row,
      Header: "操作",
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        let { id, name, isRoot } = value
        return (
          <Box alignX="center">
            <Set>
              <EditButton id={id} onSubmitted={mutate} />
              <CopyButton
                id={id}
                name={name}
                onSubmitted={mutate}
                disabled={isRoot}
              />
              <DeleteButton id={id} name={name} onSubmitted={mutate} />
            </Set>
          </Box>
        )
      }
    })
  }

  return useMemo(() => columns)
}
