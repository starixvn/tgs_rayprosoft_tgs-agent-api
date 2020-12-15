import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Formik, Field, Form, useFormikContext } from "formik"
import { Spinner, Stack, Text, Box, Button, Set, Tabs as _Tabs } from "bumbag"
import { Trans, FormattedNumber } from "../../i18n"
import format from "../../date/format"
import { useTable, Table, pageCount } from "../../table"
import { Select as GameTypeSelect } from "../../game/type"
import GameProviderSelect from "../../game/providers/Select"
import { pastWeek } from "../../date/range"
import DateTimeRangeInput from "../../date/RangeInput"
import SearchButton from "../../button/SearchButton"
import { useBettings } from "./client"
import equals from "fast-deep-equal"
import dateTimeColumn from "../../table/column/dateTime"
import BettingDetail from "./BettingDetailButton"
import { Tab as _Tab, TabPanel } from "../../tab"

export const Key = "betting"

export const Tab = ({ ...props }) => {
  return (
    <_Tab tabId={Key} {...props}>
      <Trans zh="投注记录" en="Betting records" />
    </_Tab>
  )
}

export const Panel = ({ playerCode, ...props }) => {
  let { selectedId } = props
  let isActive = selectedId === Key
  let dateRange = pastWeek()
  let { state, setState, setPageIndex, setSortOrder } = useTable({
    gameType,
    gameProvider,
    from: dateRange.start,
    to: dateRange.end
  })
  let {
    pageIndex,
    pageSize,
    sortby,
    orderby,
    gameType,
    gameProvider,
    from,
    to
  } = state
  let { data = {}, isLoading, mutate } = useBettings(playerCode, {
    page: pageIndex + 1,
    rows: pageSize,
    sortby,
    orderby,
    gameType,
    gameProvider,
    from: format(from),
    to: format(to)
  })

  let columns = useColumns()

  return (
    <TabPanel tabId={Key} {...props}>
      <Stack spacing="major-2">
        {isActive && (
          <SearchBar
            gameType={gameType}
            gameProvider={gameProvider}
            start={from}
            end={to}
            search={({ start, end, ...data }) => {
              if (
                equals(
                  {
                    from: start,
                    to: end,
                    gameType: data.gameType,
                    gameProvider: data.gameProvider
                  },
                  { gameType, gameProvider, from, to }
                )
              ) {
                mutate()
              }
              setState({
                from: start,
                to: end,
                ...data
              })
            }}
            isLoading={isLoading}
          />
        )}
        <Table
          columns={columns}
          data={data?.items}
          pageCount={pageCount(pageSize, data?.totalCount)}
          setPageIndex={setPageIndex}
          setSortOrder={setSortOrder}
          initialState={state}
          isLoading={isLoading}
          foot
        />
      </Stack>
    </TabPanel>
  )
}

const SearchBar = ({
  gameType,
  gameProvider,
  start: startDate,
  end: endDate,
  search,
  isLoading,
  ...props
}) => {
  let [gameTypeState, setGameType] = useState(gameType)
  let [gameProviderState, setGameProvider] = useState(gameProvider)
  let [start, setStart] = useState(startDate)
  let [end, setEnd] = useState(endDate)

  return (
    <Set>
      <GameTypeSelect
        value={gameTypeState}
        onChange={(event) => {
          setGameType(event.target.value)
        }}
      />
      <GameProviderSelect
        value={gameProviderState}
        onChange={(event) => {
          setGameProvider(event.target.value)
        }}
      />
      <DateTimeRangeInput
        start={start}
        end={end}
        validate
        onChangeStart={(date) => {
          setStart(date)
        }}
        onChangeEnd={(date) => {
          setEnd(date)
        }}
      />
      <SearchButton
        isLoading={isLoading}
        onClick={() => {
          search({
            gameType: gameTypeState,
            gameProvider: gameProviderState,
            start,
            end
          })
        }}
      />
    </Set>
  )
}

const useColumns = () =>
  useMemo(() => [
    dateTimeColumn({
      accessor: "betTime",
      disableSortBy: true,
      Header: <Trans zh="投注时间" en="Betting Time" />,
      Footer: <Trans zh="总和" en="Total" />,
      footerCellProps: {
        textAlign: "center"
      }
    }),
    {
      accessor: "betAmount",
      Header: <Trans zh="投注金额" en="Betting Amount" />,
      headCell: {
        textAlign: "center"
      },
      Footer: (table) => (
        <FormattedNumber value={total("betAmount", table.rows)} />
      ),
      footerCellProps: {
        textAlign: "center"
      },
      disableSortBy: true,
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      }
    },
    {
      accessor: "validBetAmount",
      Header: <Trans zh="有效投注金额" en="Valid Bet Amount" />,
      Footer: (table) => (
        <FormattedNumber value={total("validBetAmount", table.rows)} />
      ),
      footerCellProps: {
        textAlign: "center"
      },
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      }
    },
    {
      accessor: "payout",
      Header: <Trans zh="派彩" en="Payout" />,
      Footer: (table) => (
        <FormattedNumber value={total("payout", table.rows)} />
      ),
      footerCellProps: {
        textAlign: "center"
      },
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      }
    },
    {
      accessor: "winLoss",
      Header: <Trans zh="输赢金额" en="Win Loss" />,
      Footer: (table) => (
        <FormattedNumber value={total("winLoss", table.rows)} />
      ),
      footerCellProps: {
        textAlign: "center"
      },
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      Cell: ({ value }) => {
        let win = value >= 0
        return (
          <Text color={win ? "success" : "danger"}>
            <FormattedNumber value={value} format="+0,0" />
          </Text>
        )
      }
    },
    {
      id: "detail",
      accessor: (row) => {
        return row
      },
      Header: <Trans zh="游戏详情" en="Game Detail" />,
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      },
      Cell: ({ value }) => {
        let { betId, betResult } = value
        return (
          <Text>
            {betId} {betResult}
          </Text>
        )
      }
    },
    {
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
          <BettingDetail record={value}>
            <Trans zh="详情" en="Detail" />
          </BettingDetail>
        )
      }
    }
  ])

const total = (field = "", rows = []) =>
  rows.reduce((total, row) => {
    let value = Reflect.get(row.values, field)
    if (Number.isFinite(value)) {
      return total + value
    }
    return total
  }, 0)
