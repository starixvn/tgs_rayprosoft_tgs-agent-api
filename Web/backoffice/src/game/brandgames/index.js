import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Formik, Form, Field, useFormikContext } from "formik"
import {
  Heading,
  Stack,
  Button,
  InputField,
  Set,
  Text,
  Box,
  Input,
  Switch
} from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { Code } from "./menu"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"
import { SearchButton } from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { useBrandGameList } from "./client"
import { useFormattedTimeString } from "../../date/format"
import { useFormattedDay } from "../../date/week"
import formatISO from "date-fns/fp/formatISO"
import { StatusMenu } from "./status"
import { EditButton } from "./edit"
import { GameTypeLabel } from "../games/GameType"
import { GamePlatformLabel } from "../games/GamePlatform"
import { GameTag, GameTagSwitch } from "../games/GameTag"
import { GameStatusToggleSwitch } from "../games/GameStatus"

export { Code } from "./menu"

export const Content = () => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return null
  return <Body />
}

export const CodeContentPair = [Code, Content]

export const Body = () => {
  let { state, setState, setPageIndex, setSortOrder } = useTable({})
  let { pageIndex, pageSize, keyword } = state
  let { data, isLoading, mutate } = useBrandGameList(toApiParams(state))

  let [displayOrders, setDisplayOrder] = useState({})
  const changeDisplayOrder = ({ id, value }) => {
    let newDisplayOrders = {
      ...displayOrders
    }
    Reflect.set(newDisplayOrders, id, value)
    setDisplayOrder(newDisplayOrders)
  }

  let columns = useColumns({ items: data?.items, changeDisplayOrder, mutate })

  return (
    <PageContent
      title={<Trans zh="游戏列表" en="Game List" />}
      actionBar={
        <SearchBar
          keyword={keyword}
          search={(data) => {
            let newState = {
              pageIndex: 0,
              ...data
            }
            if (equals(newState, { pageIndex, keyword })) {
              mutate()
            }
            setState(newState)
          }}
        />
      }
    >
      <Table
        columns={columns}
        data={data?.items}
        pageCount={pageCount(pageSize, data?.totalCount)}
        setPageIndex={setPageIndex}
        setSortOrder={setSortOrder}
        isLoading={isLoading}
      />
    </PageContent>
  )
}

const SearchBar = ({ name: keyword = "", search, isLoading, ...props }) => {
  let searchPlaceHolder = useTrans({
    zh: "关键字输入框",
    en: "Input keyword"
  })
  return (
    <Formik
      initialValues={{
        name: keyword
      }}
      onSubmit={search}
    >
      {(formProps) => (
        <form onReset={formProps.handleReset} onSubmit={formProps.handleSubmit}>
          <Set>
            <Field
              name="keyword"
              component={InputField.Formik}
              addonAfter={
                <Button onClick={() => formProps.setFieldValue("keyword", "")}>
                  X
                </Button>
              }
              placeholder={searchPlaceHolder}
              autocomplete="off"
            />
            <SearchButton isLoading={isLoading} disabled={isLoading} />
          </Set>
        </form>
      )}
    </Formik>
  )
}

const useColumns = ({ items = [], changeDisplayOrder, mutate }) => {
  let columns = [
    {
      id: "displayOrder",
      accessor: (record) => record,
      Header: <Trans zh="序" en="Order" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        let { id, displayOrder = "" } = value
        return (
          <Input
            type="number"
            size="small"
            min="0"
            minLength="1"
            width="5em"
            defaultValue={displayOrder}
            onChange={(event) => {
              let value = event.target.value
              let number = parseInt(value)
              if (Number.isFinite(number)) {
                changeDisplayOrder({
                  id,
                  value: number
                })
              }
            }}
          />
        )
      }
    },
    {
      accessor: "gameId",
      Header: <Trans zh="游戏ID" en="ID" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      accessor: "gameProviderName",
      Header: <Trans zh="厂商名称" en="Game Provider Name" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      accessor: "gameName",
      Header: <Trans zh="游戏名称" en="Game Name" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      accessor: "gameType",
      Header: <Trans zh="游戏类型" en="Game Type" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        return (
          <Text.Block whiteSpace="nowrap">
            <GameTypeLabel value={value} />
          </Text.Block>
        )
      }
    },
    {
      accessor: "platform",
      Header: <Trans zh="支持设备" en="Platform" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        return (
          <Text.Block whiteSpace="nowrap">
            <GamePlatformLabel value={value} />
          </Text.Block>
        )
      }
    },
    {
      accessor: "isHot",
      Header: <Trans zh="热门游戏" en="Hot" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        return (
          <Text.Block whiteSpace="nowrap">
            <GameTagSwitch
              tag={GameTag.Hot}
              checked={value}
              onSubmitted={mutate}
            />
          </Text.Block>
        )
      }
    },
    {
      accessor: "isNew",
      Header: <Trans zh="最新游戏" en="New" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        return (
          <Text.Block whiteSpace="nowrap">
            <GameTagSwitch
              tag={GameTag.New}
              checked={value}
              onSubmitted={mutate}
            />
          </Text.Block>
        )
      }
    },
    {
      accessor: "isRecommend",
      Header: <Trans zh="推荐游戏" en="Recommend" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        return (
          <Text.Block whiteSpace="nowrap">
            <GameTagSwitch
              tag={GameTag.Recommend}
              checked={value}
              onSubmitted={mutate}
            />
          </Text.Block>
        )
      }
    },
    {
      accessor: "status",
      Header: <Trans zh="状态" en="Status" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        return (
          <Box alignX="center">
            <GameStatusToggleSwitch value={value} />
          </Box>
        )
      }
    }
  ]

  //TODO: auth
  let hasAnyMenu = useHasSubMenuByRoute()
  if (hasAnyMenu) {
    columns.push({
      id: "actions",
      disableSortBy: true,
      Header: <Trans zh="操作" en="Actions" />,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      accessor: (row) => {
        return row
      },
      Cell: ({ value }) => {
        return (
          <Set orientation="vertical" alignX="center">
            <EditButton record={value} onSubmitted={mutate} />
          </Set>
        )
      }
    })
  }

  return useMemo(() => columns, [JSON.stringify(items)])
}
