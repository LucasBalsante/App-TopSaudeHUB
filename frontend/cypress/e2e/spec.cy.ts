describe('My First Test', () => {
  it('Visits the initial project page', () => {
    cy.visit('/');
    cy.get('app-root').should('exist');
    cy.get('app-root').find('section.home-page').should('exist');
    cy.get('body').should('not.contain.text', 'Hello, frontend');
  });
});
