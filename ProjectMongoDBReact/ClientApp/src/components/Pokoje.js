import React, { Component } from 'react';
import { Button, Form } from 'react-bootstrap';
import axios from 'axios';
import $ from 'jquery';
import authService from './api-authorization/AuthorizeService'
import 'regenerator-runtime/runtime'

export class Pokoje extends Component {
    static displayName = Pokoje.name;

  constructor(props) {
    super(props);
      this.state = { pokoje: [], loading: true, show: false };

      this.onInputchange = this.onInputchange.bind(this);
      this.handleChecked = this.handleChecked.bind(this);
    }


  componentDidMount() {
    this.populatePokojData();
    }

    onInputchange(event) {
        this.setState({
            [event.target.name]: event.target.value
        });
    }

    handleChecked = () => {
        this.setState({ show: !this.state.show });
        console.log(this.state.show);
    }
  

  static renderPokojeTable(pokojeList,showM) {
      return (
          this.toggle_button = [],
          <table className='table table-striped' aria-labelledby="tabelLabel">
              <thead>
                  <tr>
                      <th>Nr. pokoju</th>
                      <th>Nazwa</th>
                      <th>Ile osob</th>
                      <th>Cena</th>
                      <th style={showM === false ? { display: 'none' } : {}}>Usun</th>
                      <th style={showM === true ? { display: 'none' } : {}}>Rezerwacja</th>
                  </tr>
              </thead>
              <tbody>
                  {pokojeList.map(pokoj =>
                      <tr key={pokoj.id}>

                          <td>{pokoj.nr_pokoju}</td>

                          <td><a href={'/pokojDetails/' + pokoj.id}>{pokoj.nazwa}</a></td>

                          <td>{pokoj.ile_osob}</td>

                          <td>{pokoj.cena}</td>
                              
                          <td style={showM === false ? { display: 'none' } : {} }><Button variant="outline-danger" size="sm" value={pokoj.id} onClick={deletePokoj}>Usun</Button></td>

                          <td style={showM === true ? { display: 'none' } : {}}><Button variant="outline-success" href={'/addRezerwacja/' + pokoj.id}>Zarezerwuj</Button></td>
                          
              </tr>
                  )} 
        </tbody>
          </table>


      );
  }

  render() {
    let contents = this.state.loading
        ? <p><em>Ladowanie tresci...</em></p>
        : Pokoje.renderPokojeTable(this.state.pokoje, this.state.show);

    return (
      <div>
            <h1 id="tabelLabel" >Lista pokoi</h1>
            <Button variant="success" href="/addPokoj">
                Dodaj pokoj
                </Button>
            <br /><br />
            <Button onClick={this.handleChecked}> Modul edycyjny
                </Button>
            <br />
            <br />
        {contents}
        </div>
    );
  }

  async populatePokojData() {
    const token = await authService.getAccessToken();
    const response = await fetch('pokoj/GetPokoje', {
      headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
    });
    const data = await response.json();
      this.setState({ pokoje: data, loading: false });
    }

}
////////////////////////////////
const deletePokoj = (event) => {
    let a = deleteData(event.target.value);
}

async function deleteData(idPokoj) {
    const token = await authService.getAccessToken();

    const [isAuthenticated, user] = await Promise.all([authService.isAuthenticated(), authService.getUser()]);


    axios.delete(`/pokoj/Delete/` + idPokoj + '/' + user.name,
        {
            headers: {
                Authorization: `Bearer ${token}`
            },
            data: {
                'id': idPokoj
            }

        }).then(window.location.reload());



    return 0;
}
////////////////////////////////////
const editPokoj = (event) => {
    let e = editData(event.target.value);
}

async function editData(idPokoj) {
    const nrPokoju = document.getElementById(idPokoj + '_nr_pokoju').value;
    const nazwa = document.getElementById(idPokoj + '_nazwa').value;
    const ile_osob = document.getElementById(idPokoj + '_ile_osob').value;
    const cena = document.getElementById(idPokoj + '_cena').value;


    const PokojM = {  //obiekt do przesłania
        'Id': idPokoj ,
        'nr_pokoju': parseInt(nrPokoju, 10),
        'nazwa': nazwa ,
        'ile_osob': parseInt(ile_osob, 10),
        'cena': parseInt(cena, 10) 
    }

    const token = await authService.getAccessToken();

    const data = {
                id: idPokoj,
                Pokoj: PokojM
    };

    const headers = {
        'Authorization': `Bearer ${token}`
    };

    axios.put(`/pokoj/Put/` + idPokoj,
        { 'id': idPokoj, 'nr_pokoju': PokojM.nr_pokoju, 'nazwa': PokojM.nazwa, 'ile_osob': PokojM.ile_osob, 'cena': PokojM.cena },
        { headers }).catch(error => console.log(error));

    return 0;
}

